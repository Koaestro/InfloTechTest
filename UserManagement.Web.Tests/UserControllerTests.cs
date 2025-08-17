using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;
using UserManagement.Web.Controllers;
using UserManagement.Web.Enums;
using UserManagement.Web.Models.Users;
using FluentAssertions;

namespace UserManagement.Web.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userService;
    private UsersController _controller;
    private List<User> _users;

    public UserControllerTests()
    {
        _userService = new Mock<IUserService>();
        _controller = new UsersController(_userService.Object);
        SetupUsers(); // sets up _users and mocks
    }

    private void SetupUsers()
    {
        _users = new List<User>
        {
            new User
            {
                Id = 1,
                Forename = "Johnny",
                Surname = "User",
                Email = "juser@example.com",
                IsActive = true,
                DateOfBirth = new DateTime(1990, 1, 1)
            },
            new User
            {
                Id = 2,
                Forename = "Jane",
                Surname = "Inactive",
                Email = "jane@example.com",
                IsActive = false,
                DateOfBirth = new DateTime(1985, 5, 5)
            }
        };

        _userService.Setup(s => s.GetAll()).Returns(_users.Select(u => new UserReadDto(u)));

        _userService.Setup(s => s.FilterByActive(true))
            .Returns(_users.Where(u => u.IsActive).Select(u => new UserReadDto(u)).ToList());

        _userService.Setup(s => s.FilterByActive(false))
            .Returns(_users.Where(u => !u.IsActive).Select(u => new UserReadDto(u)).ToList());


        _userService.Setup(s => s.GetUser(It.IsAny<long>()))
            .ReturnsAsync((long id) => _users.Select(u => new UserReadDto(u)).First(u => u.Id == id));

        _userService.Setup(s => s.DeleteUser(It.IsAny<long>()))
            .Returns((long id) =>
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    _users.Remove(user);
                }
                return Task.CompletedTask;
            });


        _userService.Setup(s => s.CreateUser(It.IsAny<UserWriteDto>()))
            .ReturnsAsync(_users.First().Id);

        _userService.Setup(s => s.UpdateUser(It.IsAny<UserWriteDto>()))
            .ReturnsAsync((UserWriteDto dto) =>
                _users.Select(u => new UserReadDto(u)).First(u => u.Id == dto.Id));
    }

    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        var result = _controller.List();
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(_users);
    }

    [Fact]
    public void List_WhenFilteredByActive_ModelMustOnlyContainActiveUsers()
    {
        var result = _controller.List(UserFilter.Active);
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().AllSatisfy(i => i.IsActive.Should().BeTrue());
    }

    [Fact]
    public void List_WhenFilteredByInactive_ModelMustOnlyContainInactiveUsers()
    {
        var result = _controller.List(UserFilter.Inactive);
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().AllSatisfy(i => i.IsActive.Should().BeFalse());
    }

    [Fact]
    public void Create_Get_ShouldReturnView()
    {
        var result = _controller.Create();
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Create_Post_ValidModel_ShouldRedirectToRead()
    {
        var viewModel = new UserCreateViewModel
        {
            Forename = "Jane",
            Surname = "Doe",
            Email = "jane@example.com",
            IsActive = true,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var result = await _controller.Create(viewModel);

        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be(nameof(UsersController.Read));
        redirectResult.RouteValues["userId"].Should().Be(1);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ShouldReturnSameView()
    {
        _controller.ModelState.AddModelError("Forename", "Required");

        var viewModel = new UserCreateViewModel();

        var result = await _controller.Create(viewModel);

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().Be(viewModel);
    }

    [Fact]
    public async Task Read_ValidId_ShouldReturnUserViewModel()
    {
        var result = await _controller.Read(1);

        result.ViewData.Model.Should().BeOfType<UserReadViewModel>();
        var vm = result.Model as UserReadViewModel;
        vm.Should().NotBeNull();
        vm!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Read_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidUserId = 999;

        _userService
            .Setup(s => s.GetUser(invalidUserId))
            .ThrowsAsync(new Exception($"User with Id {invalidUserId} not found"));

        // Act
        Func<Task> act = async () => await _controller.Read(invalidUserId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage($"User with Id {invalidUserId} not found");
    }


    [Fact]
    public async Task Update_Get_ValidId_ShouldReturnUpdateViewModel()
    {
        var result = await _controller.Update(1);

        result.Model.Should().BeOfType<UserUpdateViewModel>()
            .And.Match<UserUpdateViewModel>(u => u.Email == "juser@example.com");
    }

    [Fact]
    public async Task Update_Post_ValidModel_ShouldRedirectToRead()
    {
        var updateViewModel = new UserUpdateViewModel
        {
            Forename = "Updated",
            Surname = "User",
            Email = "updated@example.com",
            IsActive = true,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var result = await _controller.Update(1, updateViewModel);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.RouteValues["userId"].Should().Be(1);
    }

    [Fact]
    public async Task Update_Post_InvalidModel_ShouldReturnSameView()
    {
        _controller.ModelState.AddModelError("Email", "Required");

        var viewModel = new UserUpdateViewModel();

        var result = await _controller.Update(1, viewModel);

        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(viewModel);
    }

    [Fact]
    public async Task Delete_ValidId_ShouldRedirectToList()
    {
        var result = await _controller.Delete(1);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be(nameof(UsersController.List));
    }
}

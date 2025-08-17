using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    private readonly Mock<IDataContext> _dataContext;
    private readonly Mock<ILogService> _logService;
    private UserService _userService;
    private List<User> _users;
    private long _currentId;

    private UserService CreateService() => new(_dataContext.Object, _logService.Object);

    public UserServiceTests()
    {
        // This constructor runs before each test ensuring a fresh setup.
        _dataContext = new Mock<IDataContext>();
        _logService = new Mock<ILogService>();
        _userService = CreateService();
        SetupUsers();  // initializes _users and mocks IDataContext methods
    }

    private void SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Forename = forename,
                    Surname = surname,
                    Email = email,
                    IsActive = isActive,
                    DateOfBirth = new DateTime(1990, 1, 1)
                },

                new User
                {
                    Id = 2,
                    Forename = "Jane",
                    Surname = "Doe",
                    Email = "jdoe@example.com",
                    IsActive = false,
                    DateOfBirth = new DateTime(1985, 5, 5)
                }
            };

        _currentId = _users.Max(u => u.Id);

        _dataContext.Setup(dc => dc.GetAll<User>())
            .Returns(() => _users.AsQueryable());

        _dataContext.Setup(dc => dc.GetById<User>(It.IsAny<long>()))
            .ReturnsAsync((long id) => _users.FirstOrDefault(u => u.Id == id));

        _dataContext.Setup(dc => dc.CreateAsync<User>(It.IsAny<User>()))
            .Returns<User>(u =>
            {
                // Simulate DB auto-increment id
                _currentId++;
                u.Id = _currentId;
                _users.Add(u);
                return Task.CompletedTask;
            });

        _dataContext.Setup(dc => dc.UpdateAsync<User>(It.IsAny<User>()))
            .Returns<User>(u =>
            {
                var index = _users.FindIndex(x => x.Id == u.Id);
                if (index != -1)
                    _users[index] = u;
                return Task.CompletedTask;
            });

        _dataContext.Setup(dc => dc.DeleteAsync(It.IsAny<User>()))
            .Returns<User>(u =>
            {
                _users.Remove(u);
                return Task.CompletedTask;
            });

        _dataContext.Setup(dc => dc.Delete(It.IsAny<User>()))
            .Callback<User>(u => _users.Remove(u));
    }

    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var users = _users.Select(u => new UserReadDto(u));

        // Act: Invokes the method under test with the arranged parameters.
        var result = _userService.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(users);
    }


    [Fact]
    public void FilterByActive_ShouldReturnOnlyActiveUsers()
    {
        var expected = _users.Where(u => u.IsActive).Select(u => new UserReadDto(u));

        var result = _userService.FilterByActive(true);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void FilterByNotActive_ShouldReturnOnlyNotActiveUsers()
    {
        var expected = _users.Where(u => !u.IsActive).Select(u => new UserReadDto(u));

        var result = _userService.FilterByActive(false);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetUser_WhenUserExists_ReturnsUserDto()
    {
        var user = _users.First();

        var result = await _userService.GetUser(user.Id);

        result.Should().BeEquivalentTo(new UserReadDto(user));
    }

    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ThrowsException()
    {
        Func<Task> act = async () => await _userService.GetUser(123);

        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("User with Id 123 not found");
    }

    [Fact]
    public async Task CreateUser_ShouldCreateUserAndReturnId()
    {
        var dto = new UserWriteDto
        {
            Forename = "John",
            Surname = "Doe",
            Email = "john@example.com",
            IsActive = true,
            DateOfBirth = new DateTime(1990, 1, 1)
        };


        var result = await _userService.CreateUser(dto);

        result.Should().Be(_users.Count);
    }

    [Fact]
    public async Task UpdateUser_WhenUserExists_ShouldUpdateUser()
    {
        var user = _users.First();

        var updatedDto = new UserWriteDto
        {
            Id = user.Id,
            Forename = "Updated",
            Surname = "Name",
            Email = "updated@example.com",
            IsActive = false,
            DateOfBirth = new DateTime(1995, 5, 5)
        };

        var result = await _userService.UpdateUser(updatedDto);

        result.Should().BeEquivalentTo(updatedDto, options =>
            options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task UpdateUser_WhenUserNotFound_ShouldThrow()
    {
        var dto = new UserWriteDto { Id = 999 };

        Func<Task> act = async () => await _userService.UpdateUser(dto);

        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("User with ID 999 not found.");
    }

    [Fact]
    public async Task UpdateUser_WithoutId_ShouldThrow()
    {
        var dto = new UserWriteDto(); // Id is null

        Func<Task> act = async () => await _userService.UpdateUser(dto);

        await act.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("User ID must be provided for update.");
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ShouldDelete()
    {
        var userToDelete = _users.First();

        await _userService.DeleteUser(userToDelete.Id);

        var newUsers = _userService.GetAll();
        
        newUsers.Should().NotContain(u => u.Id == userToDelete.Id);
    }

    [Fact]
    public async Task DeleteUser_WhenUserDoesNotExist_ShouldThrow()
    {
        Func<Task> act = async () => await _userService.DeleteUser(123);

        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("User with ID 123 not found.");
    }
}

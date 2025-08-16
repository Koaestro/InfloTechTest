using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;
using UserManagement.Web.Enums;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Controllers;

[Route("users")]
public class UsersController : Controller
{
    public static readonly string NAME = "Users";

    private readonly IUserService _userService;
    private readonly ILogService _logService;
    public UsersController(IUserService userService, ILogService logService)
    {
        _userService = userService;
        _logService = logService;
    }

    [HttpGet("")]
    public ViewResult List(UserFilter? filter = null)
    {
        try
        {

            var users = filter switch
            {
                UserFilter.Active => _userService.FilterByActive(true),
                UserFilter.Inactive => _userService.FilterByActive(false),
                UserFilter.All => _userService.GetAll(),
                _ => _userService.GetAll()
            };

            var items = users.Select(p => new UserListItemViewModel
            {
                Id = p.Id,
                Forename = p.Forename,
                Surname = p.Surname,
                Email = p.Email,
                IsActive = p.IsActive
            });

            var model = new UserListViewModel
            {
                Items = items.ToList()
            };

            return View(model);
        } catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    [HttpGet("create")]
    public ViewResult Create()
    {
        try
        {
            return View();
        } catch(Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(UserCreateViewModel user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                // Return the same view with validation errors
                return View(user);
            }

            UserWriteDto userDto = new()
            {
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                IsActive = user.IsActive,
                DateOfBirth = user.DateOfBirth
            };

            var userId = await _userService.CreateUser(userDto);

            await _logService.LogAsync(new LogWriteDto(ActionType.Create, EntityType.User, userId, $"Created User {userId}"));

            return RedirectToAction(nameof(Read), new { userId });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

    }

    [HttpGet("{userId}")]
    public async Task<ViewResult> Read(long userId)
    {
        try
        {
            var user = await _userService.GetUser(userId);

            UserReadViewModel vm = new()
            {
                Id = user.Id,
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                IsActive = user.IsActive,
                DateOfBirth = user.DateOfBirth
            };

            return View(vm);
        }
        catch (Exception ex)
        {
            throw;
        }
    }


    [HttpGet("update/{userId}")]
    public async Task<ViewResult> Update(long userId)
    {
        try
        {
            var user = await _userService.GetUser(userId);

            UserUpdateViewModel viewModel = new()
            {
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                IsActive = user.IsActive,
                DateOfBirth = user.DateOfBirth
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPost("update/{userId}")]
    public async Task<IActionResult> Update(long userId, UserUpdateViewModel user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                // Return the same view with validation errors
                return View(user);
            }

            var userUpdate = new UserWriteDto()
            {
                Id = userId,
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                IsActive = user.IsActive,
                DateOfBirth = user.DateOfBirth
            };

            var updatedUser = await _userService.UpdateUser(userUpdate);

            UserUpdateViewModel viewModel = new()
            {
                Forename = updatedUser.Forename,
                Surname = updatedUser.Surname,
                Email = updatedUser.Email,
                IsActive = updatedUser.IsActive,
                DateOfBirth = updatedUser.DateOfBirth
            };

            return RedirectToAction(nameof(Read), new { userId = updatedUser.Id });
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPost("delete/{userId}")]
    public async Task<IActionResult> Delete(long userId)
    {
        try
        {
            await _userService.DeleteUser(userId);
            return RedirectToAction(nameof(List));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return View("Error");
        }
    }
}

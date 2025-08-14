using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;
using UserManagement.Web.Enums;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    public const string NAME = "users";

    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

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
                Id = p.Id ?? -1,
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

            UserDto userDto = new()
            {
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                IsActive = user.IsActive,
                DateOfBirth = user.DateOfBirth
            };

            var userId = await _userService.CreateUser(userDto);

            return RedirectToAction(nameof(Read), new { userId });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

    }

    [HttpGet("{userId}")]
    public ViewResult Read(long userId)
    {
        try
        {
            var user = _userService.GetAll().Where(u => u.Id == userId).FirstOrDefault();

            UserReadViewModel vm = new()
            {
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
    public ViewResult Update(long userId)
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            throw;
        }
    }


    [HttpGet("delete/{userId}")]
    public ViewResult Delete(long userId)
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

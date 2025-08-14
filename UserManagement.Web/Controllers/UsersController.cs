using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Enums;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet("")]
    public ViewResult List(UserFilter? filter)
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
    }


    public ViewResult Create(UserDto)
    {
        return View();
    }

    public ViewResult Read(UserFilter? filter)
    {
        return View();
    }

    public ViewResult Update(UserFilter? filter)
    {
        return View();
    }

    public ViewResult Delete(UserFilter? filter)
    {
        return View();
    }
}

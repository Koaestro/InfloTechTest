using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;
using UserManagement.Web.Enums;
using UserManagement.Web.Models.Logs;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Controllers;

[Route("logs")]
public class LogsController : Controller
{
    public static readonly string NAME = "Logs";

    private readonly ILogService _logService;
    public LogsController(ILogService logService) => _logService = logService;

    [HttpGet("")]
    public async Task<ViewResult> List()
    {
        try
        {

            var logs = await _logService.GetAll();
            var items = logs.Select(l => new LogListItemViewModel
            {
                Id = l.Id,
                ActionType = l.ActionType,
                EntityType = l.EntityType,
                EntityRef = l.EntityRef,
                Details = l.Details,
                From = l.From,
                To = l.To,
                By = l.By,
                At = l.At
            });

            var model = new LogListViewModel
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
    /*
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

            return RedirectToAction(nameof(Read), new { userId });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

    }*/
}

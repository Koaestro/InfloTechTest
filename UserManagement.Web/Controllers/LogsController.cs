using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;
using UserManagement.Web.Enums;
using UserManagement.Web.Extensions;
using UserManagement.Web.Models;
using UserManagement.Web.Models.Logs;
using UserManagement.Web.Models.Users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

    [HttpPost("get-logs")]
    public async Task<IActionResult> GetLogs(IDataTablesRequest request)
    {
        try
        {
            // Manually read actionType and entityType from the form or query string
            var actionTypeValue = HttpContext.Request.Form["actionType"].FirstOrDefault();
            var entityTypeValue = HttpContext.Request.Form["entityType"].FirstOrDefault();

            int? actionType = null;
            int? entityType = null;

            if (int.TryParse(actionTypeValue, out var at))
                actionType = at;

            if (int.TryParse(entityTypeValue, out var et))
                entityType = et;

            var logs = await _logService.GetAll();

            if (actionType.HasValue)
                logs = logs.Where(l => (int)l.ActionType == actionType.Value);

            if (entityType.HasValue)
                logs = logs.Where(l => (int)l.EntityType == entityType.Value);

            var allLogs = logs.Select(l => new LogDataTableItemViewModel
            {
                Id = l.Id,
                ActionType = l.ActionType.ToString(),
                EntityType = l.EntityType.ToString(),
                EntityRef = l.EntityRef,
                Details = l.Details,
                By = l.By,
                At = l.At
            }).AsQueryable();

            allLogs = allLogs.GlobalFilterBy(request.Search, request.Columns);

            var filteredCount = allLogs.Count();

            // Apply sorting with your extension
            allLogs = allLogs.SortBy(request.Columns);

            // Paging
            var data = allLogs.Skip(request.Start).Take(request.Length).ToList();

            var response = DataTablesResponse.Create(request, logs.Count(), filteredCount, data);
            return new DataTablesJsonResult(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Error processing logs");
        }
    }


    [HttpGet("{logId}")]
    public async Task<ViewResult> Read(long logId)
    {
        try
        {
            var log = await _logService.GetLog(logId);

            LogReadViewModel vm = new()
            {
                Id = log.Id,
                ActionType = log.ActionType,
                EntityType = log.EntityType,
                EntityRef = log.EntityRef,
                Details = log.Details,
                By = log.By,
                At = log.At
            };

            // Prettify JSON

            if (log.From is not null)
            {
                vm.From = JToken.Parse(log.From).ToString();
            }

            if (log.To is not null)
            {
                vm.To = JToken.Parse(log.To).ToString();
            }

            return View(vm);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

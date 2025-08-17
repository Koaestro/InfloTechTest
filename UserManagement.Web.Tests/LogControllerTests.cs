using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;
using UserManagement.Web.Controllers;
using UserManagement.Web.Enums;
using UserManagement.Web.Models.Logs;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Tests;

public class LogControllerTests
{
    private readonly Mock<ILogService> _logService;
    private LogsController _controller;
    private List<Log> _logs;

    public LogControllerTests()
    {
        _logService = new Mock<ILogService>();
        _controller = new LogsController(_logService.Object);
        SetupLogs(); // sets up _users and mocks
    }

    private void SetupLogs()
    {
        _logs = new List<Log>
        {
            new Log
            {
                Id = 1,
                ActionType = ActionType.Create,
                EntityType = EntityType.User,
                EntityRef = 10,
                Details = "Created user",
                From = null,
                To = null,
                By = "admin",
                At = DateTime.UtcNow
            },
            new Log
            {
                Id = 2,
                ActionType = ActionType.Update,
                EntityType = EntityType.User,
                EntityRef = 20,
                Details = "Updated user",
                From = "{\"Name\":\"Old\"}",
                To = "{\"Name\":\"New\"}",
                By = "admin",
                At = DateTime.UtcNow
            }
        };

        // Mock GetAll to return LogReadDto converted from _logs
        _logService.Setup(s => s.GetAll())
            .ReturnsAsync(_logs.Select(l => new LogReadDto(l)));

        // Mock GetLog to find the Log and convert or throw if not found
        _logService.Setup(s => s.GetLog(It.IsAny<long>()))
            .ReturnsAsync((long id) =>
            {
                var log = _logs.SingleOrDefault(l => l.Id == id);
                if (log == null)
                    throw new Exception($"Log with Id {id} not found");
                return new LogReadDto(log);
            });
    }

    [Fact]
    public async Task List_ReturnsViewResult_WithLogListViewModel()
    {
        var result = await _controller.List();

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<LogListViewModel>().Subject;
        model.Items.Should().HaveCount(_logs.Count);
    }

    [Fact]
    public async Task List_ThrowsException_PropagatesException()
    {
        _logService.Setup(s => s.GetAll()).ThrowsAsync(new Exception("Service failure"));

        Func<Task> act = async () => await _controller.List();

        await act.Should().ThrowAsync<Exception>().WithMessage("Service failure");
    }

    [Fact]
    public async Task Read_ValidId_ReturnsViewResult_WithLogReadViewModel()
    {
        var validId = 1;

        var log = _logs.Select(l => new LogReadDto(l)).First(l => l.Id == validId);

        var actualViewModel = new LogReadViewModel()
        {
            Id = log.Id,
            ActionType = log.ActionType,
            EntityType = log.EntityType,
            EntityRef = log.EntityRef,
            Details = log.Details,
            From = log.From,
            To = log.To,
            By = log.By,
            At = log.At
        };

        var result = await _controller.Read(validId);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<LogReadViewModel>().Subject;

        model.Should().BeEquivalentTo(actualViewModel);
    }

    [Fact]
    public async Task Read_InvalidId_ThrowsException()
    {
        var invalidId = 999;

        Func<Task> act = async () => await _controller.Read(invalidId);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage($"Log with Id {invalidId} not found");
    }
}

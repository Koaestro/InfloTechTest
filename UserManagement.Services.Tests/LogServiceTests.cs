using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;
using UserManagement.Services.Tests;
using UserManagement.Web.Enums;

namespace UserManagement.Data.Tests;

public class LogServiceTests
{
    private readonly Mock<IDataContext> _dataContext = new();
    private readonly LogService _logService;

    // A simple in-memory log storage to simulate DB behavior
    private List<Log> _logs = new();
    private long _currentId;

    private LogService CreateService() => new(_dataContext.Object);

    public LogServiceTests()
    {
        // This constructor runs before each test ensuring a fresh setup.
        _dataContext = new Mock<IDataContext>();
        _logService = CreateService();

        _dataContext.Setup(dc => dc.GetAll<Log>()).Returns(() => new TestAsyncEnumerable<Log>(_logs));

        _dataContext.Setup(dc => dc.GetById<Log>(It.IsAny<long>()))
            .ReturnsAsync((long id) => _logs.FirstOrDefault(l => l.Id == id));

        _dataContext.Setup(dc => dc.CreateAsync<Log>(It.IsAny<Log>()))
            .Returns<Log>(log =>
            {
                _currentId++;
                log.Id = _currentId;
                _logs.Add(log);
                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Resets the in-memory logs collection and populates it with default logs.
    /// </summary>
    /// <param name="count">Number of default logs to create (default: 3)</param>
    /// <returns>The initialized logs as IQueryable</returns>
    private void SetupLogs(int count = 3)
    {
        _logs = new List<Log>();
        _currentId = 0;

        for (int i = 1; i <= count; i++)
        {
            var log = new Log
            {
                Id = i,
                EntityRef = i * 10,
                EntityType = EntityType.User,
                ActionType = (ActionType)(i % 4),
                Details = $"Log entry {i}",
                By = $"User{i}",
                At = DateTime.UtcNow.AddMinutes(-i)
            };
            _logs.Add(log);
        }

        _currentId = count;
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllLogs()
    {
        // Arrange
        SetupLogs(3);
        var expectedDtos = _logs.Select(l => new LogReadDto(l)).ToList();

        // Act
        var result = await _logService.GetAll();

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task LogAsync_ShouldAddLog()
    {
        // Arrange
        _logs.Clear();
        _currentId = 0;

        var logDto = new LogWriteDto
        {
            EntityRef = 99,
            EntityType = EntityType.User,
            ActionType = ActionType.Create,
            Details = "Created user 99",
            By = "Tester",
            From = null,
            To = null
        };

        // Act
        await _logService.LogAsync(logDto);

        // Assert
        _logs.Should().ContainSingle();
        var log = _logs.First();

        log.Should().BeEquivalentTo(logDto, options =>
            options
                .Excluding(l => l.Id)
                .Excluding(l => l.At)
                .ExcludingMissingMembers());

        log.At.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetLog_WhenLogExists_ShouldReturnLogReadDto()
    {
        // Arrange
        SetupLogs(2);
        var log = _logs[1];

        // Act
        var result = await _logService.GetLog(log.Id);

        // Assert
        result.Should().BeEquivalentTo(new LogReadDto(log));
    }

    [Fact]
    public async Task GetLog_WhenLogDoesNotExist_ShouldThrowException()
    {
        // Arrange
        SetupLogs(1);
        var nonExistentId = 999;

        // Act
        Func<Task> act = async () => await _logService.GetLog(nonExistentId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage($"Log with Id {null} not found");
    }
}

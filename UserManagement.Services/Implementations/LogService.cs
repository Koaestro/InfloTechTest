using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;

namespace UserManagement.Services.Domain.Implementations;

public class LogService : ILogService
{
    private readonly IDataContext _dataAccess;
    public LogService(IDataContext dataAccess) => _dataAccess = dataAccess;

    public async Task<IEnumerable<LogReadDto>> GetAll()
    {
        var logs = await _dataAccess.GetAll<Log>().ToListAsync();
        return logs.Select(l => new LogReadDto(l));
    }

    public async Task LogAsync(LogWriteDto logDto)
    {
        var log = new Log
        {
            EntityRef = logDto.EntityRef,
            EntityType = logDto.EntityType,
            ActionType = logDto.ActionType,
            Details = logDto.Details,
            From = logDto.From,
            To = logDto.To,
            By = logDto.By,
            At = DateTime.UtcNow
        };

        await _dataAccess.CreateAsync<Log>(log);
    }

}

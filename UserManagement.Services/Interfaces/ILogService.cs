using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Services.Dtos;

namespace UserManagement.Services.Domain.Interfaces;

public interface ILogService 
{
    /// <summary>
    /// Returns all logs
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<LogReadDto>> GetAll();

    Task LogAsync(LogWriteDto logDto);


    Task<LogReadDto> GetLog(long logId);

}

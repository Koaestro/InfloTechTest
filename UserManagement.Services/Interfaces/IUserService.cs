using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Services.Dtos;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    IEnumerable<UserDto> FilterByActive(bool isActive);
    IEnumerable<UserDto> GetAll();
    Task<long> CreateUser(UserDto userDto);
}

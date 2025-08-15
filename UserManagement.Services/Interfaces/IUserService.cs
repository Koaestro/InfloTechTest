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
    IEnumerable<UserReadDto> FilterByActive(bool isActive);
    IEnumerable<UserReadDto> GetAll();
    Task<UserReadDto> GetUser(long userId);
    Task<long> CreateUser(UserWriteDto userDto);
    Task<UserReadDto> UpdateUser(UserWriteDto userDto);
    Task DeleteUser(long userId);

}

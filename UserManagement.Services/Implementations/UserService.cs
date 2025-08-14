using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<UserDto> FilterByActive(bool isActive)
    {
        return _dataAccess.GetAll<User>().Where(u => u.IsActive == isActive).Select(u => new UserDto(u));
    }

    public IEnumerable<UserDto> GetAll() => _dataAccess.GetAll<User>().Select(u => new UserDto(u));

    public async Task<long> CreateUser(UserDto userDto)
    {
        User user = new()
        {
            Forename = userDto.Forename,
            Surname = userDto.Surname,
            Email = userDto.Email,
            IsActive = userDto.IsActive,
            DateOfBirth = userDto.DateOfBirth
        };
        await _dataAccess.CreateAsync<User>(user);

        return user.Id;
    }

}

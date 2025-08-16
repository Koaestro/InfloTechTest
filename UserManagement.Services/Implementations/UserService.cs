using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Dtos;
using UserManagement.Web.Enums;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    private readonly ILogService _logService;
    public UserService(IDataContext dataAccess, ILogService logService)
    {
        _dataAccess = dataAccess;
        _logService = logService;
    }

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<UserReadDto> FilterByActive(bool isActive)
    {
        return _dataAccess.GetAll<User>().Where(u => u.IsActive == isActive).Select(u => new UserReadDto(u));
    }

    public IEnumerable<UserReadDto> GetAll() => _dataAccess.GetAll<User>().Select(u => new UserReadDto(u));

    public async Task<UserReadDto> GetUser(long userId)
    {
        var user = await _dataAccess.GetById<User>(userId);

        if (user is null)
        {
            throw new Exception($"User with Id {userId} not found");
        } else
        {
            return new UserReadDto(user);
        }

    }

    public async Task<long> CreateUser(UserWriteDto userDto)
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

        await _logService.LogAsync(new(ActionType.Create, EntityType.User, user.Id, $"Created User {user.Id}"));

        return user.Id;
    }

    public async Task<UserReadDto> UpdateUser(UserWriteDto userDto)
    {
        if (userDto.Id == null)
            throw new ArgumentException("User ID must be provided for update.");

        var user = await _dataAccess.GetById<User>(userDto.Id.Value);

        if (user == null)
            throw new Exception($"User with ID {userDto.Id.Value} not found.");

        user.Forename = userDto.Forename;
        user.Surname = userDto.Surname;
        user.Email = userDto.Email;
        user.IsActive = userDto.IsActive;
        user.DateOfBirth = userDto.DateOfBirth;

        await _dataAccess.UpdateAsync<User>(user);

        await _logService.LogAsync(new(ActionType.Update, EntityType.User, user.Id, $"Updated User {user.Id}", userDto, new UserReadDto(user)));

        return new UserReadDto(user);
    }

    public async Task DeleteUser(long userId)
    {
        var user = await _dataAccess.GetById<User>(userId);
        if (user == null)
            throw new Exception($"User with ID {userId} not found.");

        await _logService.LogAsync(new(ActionType.Delete, EntityType.User, user.Id, $"Deleted User {user.Id}"));
        await _dataAccess.DeleteAsync(user);
    }


}

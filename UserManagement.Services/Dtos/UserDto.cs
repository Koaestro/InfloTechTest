using System;
using UserManagement.Models;

namespace UserManagement.Services.Dtos;
public class UserDto
{
    public UserDto()
    {

    }
    public UserDto(User user)
    {
        Id = user.Id;
        Forename = user.Forename;
        Surname = user.Surname;
        Email = user.Email;
        IsActive = user.IsActive;
        DateOfBirth = user.DateOfBirth;
    }

    public long? Id { get; set; }
    public string Forename { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }

    public DateTime DateOfBirth { get; set; }
}

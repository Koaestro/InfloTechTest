using System;

namespace UserManagement.Web.Models.Users;

public class UserReadViewModel
{
    public long Id { get; set; }
    public string Forename { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime DateOfBirth { get; set; }
}


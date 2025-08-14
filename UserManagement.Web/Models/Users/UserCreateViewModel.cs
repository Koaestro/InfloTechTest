using System;
using System.ComponentModel.DataAnnotations;
using UserManagement.Web.Validation;

namespace UserManagement.Web.Models.Users;

public class UserCreateViewModel
{
    [Required(ErrorMessage = "Forename is required.")]
    [StringLength(50, ErrorMessage = "Forename can't be longer than 50 characters.")]
    public string Forename { get; set; } = default!;

    [Required(ErrorMessage = "Surname is required.")]
    [StringLength(50, ErrorMessage = "Surname can't be longer than 50 characters.")]
    public string Surname { get; set; } = default!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = default!;

    public bool IsActive { get; set; }

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    [DateOfBirth]
    public DateTime DateOfBirth { get; set; }
}


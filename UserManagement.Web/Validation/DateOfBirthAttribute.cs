using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Validation
{
    public class DateOfBirthAttribute : ValidationAttribute
    {
        public DateOfBirthAttribute()
        {
            ErrorMessage = "Date of birth cannot be in the future.";
        }

        public override bool IsValid(object? value)
        {
            if (value is not DateTime date)
            {
                return true;
            }

            return date <= DateTime.Today;
        }
    }
}


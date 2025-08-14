using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Web.Validation;

namespace UserManagement.Web.Tests;
public class ValidationTests
{
    [Fact]
    public void DateOfBirth_ForFutureDate()
    {
        var attr = new DateOfBirthAttribute();
        var futureDate = DateTime.Today.AddDays(1);

        var result = attr.IsValid(futureDate);

        result.Should().BeFalse();
    }

    [Fact]
    public void DateOfBirth_ForPastDate()
    {
        var attr = new DateOfBirthAttribute();
        var futureDate = DateTime.Today.AddDays(-1);

        var result = attr.IsValid(futureDate);

        result.Should().BeTrue();
    }

    [Fact]
    public void DateOfBirth_ForCurrentDate()
    {
        var attr = new DateOfBirthAttribute();
        var currentDate = DateTime.Today;

        var result = attr.IsValid(currentDate);

        result.Should().BeTrue();
    }
}

using System;
using UserManagement.Web.Enums;

namespace UserManagement.Web.Models.Users;

public class LogReadViewModel
{
    public long Id { get; set; }
    public EntityType EntityType { get; set; }
    public ActionType ActionType { get; set; }
    public long EntityRef { get; set; }
    public string Details { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string By { get; set; }
    public DateTime At { get; set; }
}


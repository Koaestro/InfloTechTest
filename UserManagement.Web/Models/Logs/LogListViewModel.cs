using System;
using UserManagement.Web.Enums;

namespace UserManagement.Web.Models.Logs;

public class LogListViewModel
{
    public List<LogListItemViewModel> Items { get; set; } = new();
}

public class LogListItemViewModel
{
    public long Id { get; set; }
    public ActionType ActionType { get; set; }
    public EntityType EntityType { get; set; }
    public long EntityRef { get; set; }
    public string Details { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string? By { get; set; }
    public DateTime At { get; set; }
}

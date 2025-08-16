using System;
using UserManagement.Web.Enums;

namespace UserManagement.Web.Models.Logs;

public class LogDataTableItemViewModel
{
    public long Id { get; set; }
    public string ActionType { get; set; }
    public string EntityType { get; set; }
    public long EntityRef { get; set; }
    public string Details { get; set; }
    public string By { get; set; }
    public DateTime At { get; set; }
}

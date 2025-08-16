using System;
using System.Linq;
using UserManagement.Web.Enums;

namespace UserManagement.Web.Models.Logs;

public class LogListViewModel
{
    public List<LogListItemViewModel> Items { get; set; } = new();

    // New properties to hold enum values for dropdowns
    public IEnumerable<ActionType> ActionTypes { get { return Enum.GetValues(typeof(ActionType)).Cast<ActionType>(); } }
    public IEnumerable<EntityType> EntityTypes { get { return Enum.GetValues(typeof(EntityType)).Cast<EntityType>(); } }
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

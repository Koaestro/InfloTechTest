using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserManagement.Data.Entities;
using UserManagement.Web.Enums;

namespace UserManagement.Models;

public class Log : IEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long EntityRef { get; set; } 
    public EntityType EntityType { get; set; }
    public ActionType ActionType { get; set; }
    public string Details { get; set; } = default!;
    public string? From { get; set; } = default!;
    public string? To { get; set; } = default!;
    public string By { get; set; } = default!;
    public DateTime At { get; set; } = DateTime.UtcNow;
}

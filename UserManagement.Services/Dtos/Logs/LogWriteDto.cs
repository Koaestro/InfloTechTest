using System;
using UserManagement.Models;
using UserManagement.Web.Enums;

namespace UserManagement.Services.Dtos;
public class LogWriteDto
{
    public LogWriteDto()
    {

    }

    /// <summary>
    /// Creates a LogWriteDto with the specified arguments
    /// </summary>
    /// <param name="actionType">Type of the action</param>
    /// <param name="entityType">Type of the entity</param>
    /// <param name="entityRef">Id of the entity</param>
    /// <param name="details">Details to be logged</param>
    /// <param name="from">Original data</param>
    /// <param name="to">Updated data</param>
    public LogWriteDto(ActionType actionType, EntityType entityType, long entityRef, string details, string? from = null, string? to = null)
    {
        ActionType = actionType;
        EntityType = entityType;
        EntityRef = entityRef;
        Details = details;
        From = from;
        To = to;
        By = "System"; //TODO: change this when adding usernames
        At = DateTime.UtcNow;
    }

    public LogWriteDto(Log log)
    {
        Id = log.Id;
        EntityType = log.EntityType;
        EntityRef = log.EntityRef;
        ActionType = log.ActionType;
        Details = log.Details;
        From = log.From;
        To = log.To;
        By = log.By;
        At = log.At;
    }

    public long? Id { get; set; }
    public EntityType EntityType { get; set; }
    public ActionType ActionType { get; set; }
    public long EntityRef { get; set; }
    public string Details { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string By { get; set; }
    public DateTime At { get; set; }
}

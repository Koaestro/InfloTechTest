using System;
using UserManagement.Models;
using UserManagement.Web.Enums;

namespace UserManagement.Services.Dtos;
public class LogReadDto
{
    public LogReadDto()
    {

    }
    public LogReadDto(Log log)
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

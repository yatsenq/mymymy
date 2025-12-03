using System;
using System.ComponentModel.DataAnnotations; // <- додай цей using

namespace StayFit.DAL.Entities;

public class ActivityLog
{
    
    public int ActivityLogId { get; set; }

    public int? UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public string Status { get; set; } = "SUCCESS";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}

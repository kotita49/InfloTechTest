using System;

namespace UserManagement.Models;

public class LogEntry
{
    public int Id { get; set; }
    public long? UserId { get; set; }  
    public string Action { get; set; } = string.Empty; 
    public string Details { get; set; } = string.Empty; 
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

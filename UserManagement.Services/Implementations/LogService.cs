using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;
public class LogService : ILogService
{
    private readonly DataContext _context;
    public LogService(DataContext context) => _context = context;

    public async Task AddLogAsync(string action, string details, long? userId = null)
    {
        var log = new LogEntry
        {
            Action = action,
            Details = details,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<List<LogEntry>> GetAllLogsAsync() =>
        await _context.Logs.OrderByDescending(l => l.Timestamp).ToListAsync();

    public async Task<LogEntry?> GetLogByIdAsync(long id) =>
        await _context.Logs.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<List<LogEntry>> GetLogsForUserAsync(long userId) =>
        await _context.Logs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
}


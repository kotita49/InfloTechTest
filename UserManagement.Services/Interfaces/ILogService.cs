
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Interfaces;
public interface ILogService
{
    Task AddLogAsync(string action, string details, long? userId = null);
    Task<List<LogEntry>> GetAllLogsAsync();
    Task<LogEntry?> GetLogByIdAsync(long id);
    Task<List<LogEntry>> GetLogsForUserAsync(long userId);
}

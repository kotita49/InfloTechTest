using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Interfaces;

namespace UserManagement.Web.Controllers;
public class LogsController : Controller
{
    private readonly ILogService _logService;

    public LogsController(ILogService logService)
    {
        _logService = logService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var allLogs = await _logService.GetAllLogsAsync();
        var totalItems = allLogs.Count;

        var logs = allLogs
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new PagedResults<LogEntry>
        {
            Items = logs,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var log = await _logService.GetLogByIdAsync(id);
        if (log == null) return NotFound();
        return View(log);
    }
}

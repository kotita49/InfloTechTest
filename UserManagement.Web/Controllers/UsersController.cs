
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogService _logService;
    public UsersController(IUserService userService, ILogService logService)
    {
        _userService = userService;
        _logService = logService;

    }

    [HttpGet]
    public async Task<IActionResult> List(string filter)
    {
        var users = string.IsNullOrEmpty(filter)
        ? await _userService.GetAllAsync()
        : await _userService.FilterByActiveAsync(filter);

        var model = new UserListViewModel
        {
            Items = users.Select(MapToViewModel).ToList()
        };

        return View(model);
    }

    [HttpGet("add")]
    public IActionResult Add()
    {
        //  return a view to add a new user.
        
        return View(new UserListItemViewModel());
    }

    [HttpPost("add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(UserListItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User
        {
            Forename = model.Forename,
            Surname = model.Surname,
            Email = model.Email,
            IsActive = model.IsActive,
            DateOfBirth = model.DateOfBirth
        };
        await _userService.AddAsync(user);
        await _logService.AddLogAsync("Created", $"User {user.Forename} {user.Surname} was created.", user.Id);

        return RedirectToAction(nameof(List));
    }

    [HttpGet("view/{id}")]
    public async Task<IActionResult> View(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        var model = MapToViewModel(user);
        var logs = await _logService.GetLogsForUserAsync(id);
        ViewBag.Logs = logs;
        return View(model);
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        var model = MapToViewModel(user);
        return View(model);
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, UserListItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
       

        user.Forename = model.Forename;
        user.Surname = model.Surname;
        user.Email = model.Email;
        user.IsActive = model.IsActive;
        user.DateOfBirth = model.DateOfBirth;

        await _userService.UpdateAsync(user);
        await _logService.AddLogAsync("Updated", $"User {user.Forename} {user.Surname} was updated.", user.Id);

        return RedirectToAction(nameof(List));
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        await _userService.DeleteAsync(id);
        await _logService.AddLogAsync("Deleted", $"User {user.Forename} {user.Surname} was deleted.", id);

        return RedirectToAction(nameof(List));

    }

    private UserListItemViewModel MapToViewModel(User user) => new()
    {
        Id = user.Id,
        Forename = user.Forename,
        Surname = user.Surname,
        Email = user.Email,
        IsActive = user.IsActive,
        DateOfBirth = user.DateOfBirth
    };
}

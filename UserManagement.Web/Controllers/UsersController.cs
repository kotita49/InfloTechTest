
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

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
        return RedirectToAction(nameof(List));
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        await _userService.DeleteAsync(id);
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

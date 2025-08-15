using System;
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
    public async Task<ViewResult> List(string filter)
    {
        IEnumerable<User> users;
        if (string.IsNullOrEmpty(filter))
        {
            users = await _userService.GetAllAsync();
        }
        else if(filter.Equals("active", StringComparison.OrdinalIgnoreCase))
        {
            users = await _userService.FilterByActiveAsync(true);
        }
        else if (filter.Equals("inactive", StringComparison.OrdinalIgnoreCase))
        {
            users = await _userService.FilterByActiveAsync(false);
        }
        else
        {
            users = await _userService.GetAllAsync();
        }
        var model = new UserListViewModel
        {
            Items = users.Select(p => new UserListItemViewModel
            {
                Id = p.Id,
                Forename = p.Forename,
                Surname = p.Surname,
                Email = p.Email,
                IsActive = p.IsActive,
                DateOfBirth = p.DateOfBirth
            }).ToList()
        };

        return View(model);
    }
}

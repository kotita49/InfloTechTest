using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    Task<IEnumerable<User>> FilterByActiveAsync(string isActive);
    /// <summary>
    /// Return all users
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(long id);
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(long id);
}

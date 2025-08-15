﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public async Task<IEnumerable<User>> FilterByActiveAsync(bool isActive)
    {
        return await Task.Run(() =>
        _dataAccess.GetAll<User>()
            .Where(user => user.IsActive == isActive)
            .ToList()
            );
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await Task.Run(() => _dataAccess.GetAll<User>()
        .ToList());
    }
}

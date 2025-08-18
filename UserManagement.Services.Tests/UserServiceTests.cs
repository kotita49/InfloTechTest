using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService(out var context);
        var users = context.GetAll<User>().ToList();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAllAsync();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task FilterByActiveAsync_WhenContextReturnsActiveEntities_MustReturnSameEntities()
    {
        // Arrange
        var service = CreateService(out var context);        


        // Act
        var result = await service.FilterByActiveAsync("active");
        // Assert
        result.Should().OnlyContain(u => u.IsActive);
    }

    [Fact]
    public async Task FilterByActiveAsync_WhenContextReturnsInactiveEntities_MustReturnSameEntities()
    {
        // Arrange
        var service = CreateService(out var context);        

        // Act
        var result = await service.FilterByActiveAsync("inactive");
        // Assert
        result.Should().OnlyContain(u => !u.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_MustReturnEntity()
    {
        var service = CreateService(out var context);
        var existing = context.GetAll<User>().First();

        var result = await service.GetByIdAsync(existing.Id);

        result.Should().BeEquivalentTo(existing);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_MustReturnNull()
    {
        var service = CreateService(out var _);

        var result = await service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCalled_MustAddUser()
    {
        var service = CreateService(out var context);
        var newUser = new User
        {
            Forename = "Alice",
            Surname = "Smith",
            Email = "alice@example.com",
            IsActive = true,
            DateOfBirth = new DateTime(1995, 5, 5)
        };

        await service.AddAsync(newUser);

        var users = await context.Users.ToListAsync();
        users.Should().ContainEquivalentOf(newUser, opt => opt.Excluding(u => u.Id));
    }

    [Fact]
    public async Task UpdateAsync_WhenCalled_MustUpdateUser()
    {
        var service = CreateService(out var context);
        var user = context.Users.First();
        user.Forename = "UpdatedName";

        await service.UpdateAsync(user);

        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser?.Forename.Should().Be("UpdatedName");
    }

    [Fact]
    public async Task DeleteAsync_WhenUserExists_MustRemoveUser()
    {
        var service = CreateService(out var context);
        var user = context.Users.First();

        await service.DeleteAsync(user.Id);

        var users = await context.Users.ToListAsync();
        users.Should().NotContain(u => u.Id == user.Id);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserDoesNotExist_MustDoNothing()
    {
        var service = CreateService(out var context);
        var nonExistingId = 999;

        await service.DeleteAsync(nonExistingId);

        var users = await context.Users.ToListAsync();
        users.Should().HaveCount(12); 
    }



    private UserService CreateService(out DataContext context)
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        context = new DataContext(options);
        context.CreateAsync(new User { Forename = "Johnny", Surname = "User", Email = "juser@example.com", IsActive = true, DateOfBirth = new DateTime(1990, 1, 1) }).GetAwaiter().GetResult();
        return new UserService(context);
    }

}

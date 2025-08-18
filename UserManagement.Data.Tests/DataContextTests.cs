using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsAllEntities()
    {
        var context = CreateContext();
        var result = await context.GetAllAsync<User>();
        result.Should().NotBeEmpty()
              .And.Contain(u => u.Email == "alice@example.com");
    }

    [Fact]
    public async Task GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        await context.CreateAsync(entity);
        entity.Id.Should().BeGreaterThan(0);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity, opts => opts.Excluding(u => u.Id));
    }

    [Fact]
    public async Task GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
        await context.DeleteAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public async Task GetAll_WhenUpdated_MustIncludeUpdatedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
        entity.Forename = "Updated Name";
        await context.UpdateAsync(entity);
        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();
        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Forename.Should().Be("Updated Name");
    }

    private DataContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);

        // Seed data
        context.CreateAsync(new User { Forename = "Alice", Surname = "Tester", Email = "alice@example.com" }).GetAwaiter().GetResult();
        context.CreateAsync(new User { Forename = "Bob", Surname = "Tester", Email = "bob@example.com" }).GetAwaiter().GetResult();

        return context;
    }


}

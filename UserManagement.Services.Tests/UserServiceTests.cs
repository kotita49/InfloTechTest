using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAllAsync();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task FilterByActiveAsync_WhenContextReturnsActiveEntities_MustReturnSameEntities()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers(isActive: true);
        
        // Act
        var result = await service.FilterByActiveAsync(true);
        // Assert
        result.Should().OnlyContain(u => u.IsActive);
    }

    [Fact]
    public async Task FilterByActiveAsync_WhenContextReturnsInactiveEntities_MustReturnSameEntities()
    {
        // Arrange
        var service = CreateService();
        var users = SetupUsers(isActive: false);
        
        // Act
        var result = await service.FilterByActiveAsync(false);
        // Assert
        result.Should().OnlyContain(u => !u.IsActive);
    }

    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive
            }
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(users);

        return users;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);
}

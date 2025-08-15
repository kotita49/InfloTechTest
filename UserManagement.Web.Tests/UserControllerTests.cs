using System;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public async Task List_WhenFilterIsNull_ShouldReturnAllUsersAsync()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();
        _userService.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await controller.List(string.Empty);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task List_WhenFilterIsActive_ShouldReturnOnlyActiveUsersAsync()
    {
        // Arrange
        var controller = CreateController();
        var users = SetupUsers(isActive: true);
        _userService
            .Setup(s => s.FilterByActiveAsync(true))
            .ReturnsAsync(users);

        // Act
        var result = await controller.List("active");

        // Assert
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task List_WhenFilterIsInactive_ShouldReturnOnlyInactiveUsersAsync()
    {
        // Arrange
        var controller = CreateController();
        var users = SetupUsers(isActive: false);
        _userService
            .Setup(s => s.FilterByActiveAsync(false))
            .ReturnsAsync(users);
        // Act
        var result = await controller.List("inactive");
        // Assert
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive,
                DateOfBirth = new DateTime(1990, 1, 1)
            }
        };        

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}

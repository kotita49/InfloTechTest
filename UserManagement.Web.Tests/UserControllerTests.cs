using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserListViewModel>(viewResult.Model);
       model.Items.Should().BeEquivalentTo(users);
    }

    [Theory]
    [InlineData("active", true)]
    [InlineData("inactive", false)]
    public async Task List_Filter_ShouldReturnFilteredUsers(string filter, bool isActive)
    {
        var controller = CreateController();
        var users = SetupUsers(isActive);
        _userService.Setup(s => s.FilterByActiveAsync(filter)).ReturnsAsync(users);

        var result = await controller.List(filter);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserListViewModel>(viewResult.Model);
        model.Items.Should().BeEquivalentTo(users);
    }

    
    [Fact]
    public async Task Add_Post_ValidModel_ShouldCallServiceAndRedirect()
    {
        // Arrange
        var controller = CreateController();
        var model = SetupUserViewModel();

        // Act
        var result = await controller.Add(model);

        // Assert
        _userService.Verify(s => s.AddAsync(It.Is<User>(u =>
            u.Forename == model.Forename &&
            u.Surname == model.Surname &&
            u.Email == model.Email
        )), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        redirectResult.ActionName.Should().Be(nameof(controller.List));
    }

    [Fact]
    public async Task Add_Post_InvalidModel_ShouldReturnViewWithModel()
    {
        // Arrange
        var controller = CreateController();
        var model = SetupUserViewModel();
        controller.ModelState.AddModelError("Email", "Email is required");
        // Act
        var result = await controller.Add(model);
        // Assert
        _userService.Verify(s => s.AddAsync(It.IsAny<User>()), Times.Never);
        var viewResult = Assert.IsType<ViewResult>(result);
        viewResult.Model.Should().Be(model);
    }

    [Fact]
    public async Task Edit_Get_ExistingUser_ShouldReturnViewWithUser()
    {
        // Arrange
        var controller = CreateController();
        var user = SetupUsers()[0];
        _userService.Setup(s => s.GetByIdAsync(user.Id)).ReturnsAsync(user);
        // Act
        var result = await controller.Edit(user.Id);
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserListItemViewModel>(viewResult.Model);
        model.Forename.Should().Be(user.Forename);
        model.Surname.Should().Be(user.Surname);
        model.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Edit_Get_NonExistingUser_ShouldReturnNotFound()
    {
        var controller = CreateController();
        _userService.Setup(s => s.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((User?)null);

        var result = await controller.Edit(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_ShouldUpdateUserAndRedirect()
    {
        // Arrange
        var controller = CreateController();
        var user = SetupUsers()[0];
        var model = SetupUserViewModel();
        _userService.Setup(s => s.GetByIdAsync(user.Id)).ReturnsAsync(user);
        // Act
        var result = await controller.Edit(user.Id, model);
        // Assert
        _userService.Verify(s => s.UpdateAsync(It.Is<User>(u =>
            u.Id == user.Id &&
            u.Forename == model.Forename &&
            u.Surname == model.Surname &&
            u.Email == model.Email
        )), Times.Once);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        redirectResult.ActionName.Should().Be(nameof(controller.List));
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ShouldReturnViewWithModel()
    {
        // Arrange
        var controller = CreateController();
        var user = SetupUsers()[0];
        var model = SetupUserViewModel();
        _userService.Setup(s => s.GetByIdAsync(user.Id)).ReturnsAsync(user);
        controller.ModelState.AddModelError("Email", "Email is required");
        // Act
        var result = await controller.Edit(user.Id, model);
        // Assert
        _userService.Verify(s => s.UpdateAsync(It.IsAny<User>()), Times.Never);
        var viewResult = Assert.IsType<ViewResult>(result);
        viewResult.Model.Should().Be(model);
    }

    [Fact]
    public async Task Delete_ExistingUser_ShouldCallServiceAndRedirect()
    {
        // Arrange
        var controller = CreateController();
        var user = SetupUsers()[0];
        _userService.Setup(s => s.GetByIdAsync(user.Id)).ReturnsAsync(user);
        // Act
        var result = await controller.Delete(user.Id);
        // Assert
        _userService.Verify(s => s.DeleteAsync(user.Id), Times.Once);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        redirectResult.ActionName.Should().Be(nameof(controller.List));
    }

    [Fact]
    public async Task Delete_NonExistingUser_ShouldReturnNotFound()
    {
        var controller = CreateController();
        _userService.Setup(s => s.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((User?)null);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundResult>(result);
        _userService.Verify(s => s.DeleteAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task View_ExistingUser_ShouldReturnView()
    {
        var controller = CreateController();
        var user = SetupUsers()[0];
        _userService.Setup(s => s.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var result = await controller.View(user.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserListItemViewModel>(viewResult.Model);
        model.Forename.Should().Be(user.Forename);
    }

    [Fact]
    public async Task View_NonExistingUser_ShouldReturnNotFound()
    {
        var controller = CreateController();
        _userService.Setup(s => s.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((User?)null);

        var result = await controller.View(1);

        Assert.IsType<NotFoundResult>(result);
    }



    private User[] SetupUsers(bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = "Johnny",
                Surname = "User",
                Email = "juser@example.com",
                IsActive = isActive,
                DateOfBirth = new DateTime(1990, 1, 1)
            }
        };        

        return users;
    }

    private UserListItemViewModel SetupUserViewModel() => new()
    {
        Forename = "Test",
        Surname = "User",
        Email = "test@example.com",
        IsActive = true,
        DateOfBirth = new DateTime(1990, 1, 1)
    };

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}

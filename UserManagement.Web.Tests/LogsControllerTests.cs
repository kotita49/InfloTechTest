using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Controllers;

namespace UserManagement.Web.Tests;
public class LogsControllerTests
{
    private readonly Mock<ILogService> _mockLogService;
    private readonly LogsController _controller;

    public LogsControllerTests()
    {
        _mockLogService = new Mock<ILogService>();
        _controller = new LogsController(_mockLogService.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithPagedResults()
    {
        // Arrange
        var logs = new List<LogEntry>
        {
            new LogEntry { Id = 1, Action = "Login", Details = "User logged in", Timestamp = DateTime.UtcNow },
            new LogEntry { Id = 2, Action = "Logout", Details = "User logged out", Timestamp = DateTime.UtcNow.AddMinutes(-5) }
        };

        _mockLogService.Setup(s => s.GetAllLogsAsync()).ReturnsAsync(logs);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<PagedResults<LogEntry>>(viewResult.Model);

        Assert.Equal(2, model.Items.Count);
        Assert.Equal(1, model.Page);       
        Assert.Equal(10, model.PageSize);  
        Assert.Equal(2, model.TotalItems);
        Assert.Equal(1, model.TotalPages);
        Assert.Equal("Login", model.Items.First().Action);
    }

    [Fact]
    public async Task Index_PaginatesLogs()
    {
        // Arrange
        var logs = Enumerable.Range(1, 50).Select(i =>
            new LogEntry { Id = i, Action = $"Action {i}", Timestamp = DateTime.UtcNow.AddMinutes(-i) }
        ).ToList();

        _mockLogService.Setup(s => s.GetAllLogsAsync()).ReturnsAsync(logs);

        // Act
        var result = await _controller.Index(page: 2, pageSize: 10);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<PagedResults<LogEntry>>(viewResult.Model);

        Assert.Equal(10, model.Items.Count);  
        Assert.Equal(2, model.Page);          
        Assert.Equal(50, model.TotalItems);   
        Assert.Equal(5, model.TotalPages);   
        Assert.Equal("Action 11", model.Items.First().Action);
    }


    [Fact]
    public async Task Details_ReturnsView_WhenLogExists()
    {
        // Arrange
        var log = new LogEntry { Id = 1, Action = "Test", Details = "Some details" };
        _mockLogService.Setup(s => s.GetLogByIdAsync(1)).ReturnsAsync(log);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<LogEntry>(viewResult.Model);
        Assert.Equal("Test", model.Action);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenLogDoesNotExist()
    {
        // Arrange
        _mockLogService.Setup(s => s.GetLogByIdAsync(99)).ReturnsAsync((LogEntry?)null);

        // Act
        var result = await _controller.Details(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

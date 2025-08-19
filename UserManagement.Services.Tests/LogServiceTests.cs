using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Implementations;


namespace UserManagement.Services.Tests;
public class LogServiceTests
{
    private DataContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
            .Options;

        return new DataContext(options);
    }

    [Fact]
    public async Task AddLogAsync_ShouldAddLog()
    {
        // Arrange
        var context = GetInMemoryContext();
        var service = new LogService(context);

        // Act
        await service.AddLogAsync("TestAction", "TestDetails", 123);

        // Assert
        var logs = await context.Logs.ToListAsync();
        Assert.Single(logs);
        Assert.Equal("TestAction", logs[0].Action);
        Assert.Equal("TestDetails", logs[0].Details);
        Assert.Equal(123, logs[0].UserId);
    }

    [Fact]
    public async Task GetAllLogsAsync_ShouldReturnLogsInDescendingOrder()
    {
        var context = GetInMemoryContext();
        var service = new LogService(context);

        // Arrange
        await context.Logs.AddRangeAsync(
            new LogEntry { Action = "First", Details = "First log", Timestamp = DateTime.UtcNow.AddMinutes(-10) },
            new LogEntry { Action = "Second", Details = "Second log", Timestamp = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetAllLogsAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Second", result.First().Action); // latest first
    }

    [Fact]
    public async Task GetLogByIdAsync_ShouldReturnCorrectLog()
    {
        var context = GetInMemoryContext();
        var service = new LogService(context);

        var log = new LogEntry { Action = "Lookup", Details = "Find by Id" };
        context.Logs.Add(log);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetLogByIdAsync(log.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Lookup", result.Action);
    }

    [Fact]
    public async Task GetLogsForUserAsync_ShouldReturnOnlyUserLogs()
    {
        var context = GetInMemoryContext();
        var service = new LogService(context);

        await context.Logs.AddRangeAsync(
            new LogEntry { Action = "User1Action", UserId = 1, Timestamp = DateTime.UtcNow },
            new LogEntry { Action = "User2Action", UserId = 2, Timestamp = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetLogsForUserAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("User1Action", result[0].Action);
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Data;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;
using Westwind.AspNetCore.Markdown;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddDataAccess()
    .AddDomainServices()
    .AddMarkdown()
    .AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseInMemoryDatabase("UserManagement"));
builder.Services.AddScoped<IDataContext>(sp => sp.GetRequiredService<DataContext>());

// Register UserService
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

app.UseMarkdown();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

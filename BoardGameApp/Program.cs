using BoardGameApp.Data;
using BoardGameApp.Data.Models;
using BoardGameApp.Data.Seeding.Utilities;
using BoardGameApp.Services.Core;
using BoardGameApp.Services.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>() 
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<BoardGameAppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<BoardgameUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<BoardGameAppDbContext>(); 

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IBoardGameService, BoardGameService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameAppDbContext>();
    
    DataProcessor.ImportBoardGames(dbContext);
    DataProcessor.ImportCategories(dbContext);
    DataProcessor.ImportBoardGameCategories(dbContext);
    DataProcessor.ImportCities(dbContext);
    DataProcessor.ImportClubs(dbContext);
    DataProcessor.ImportClubBoardGames(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "boardgames",
    pattern: "BoardGames/{action=Index}/{id?}",
    defaults: new { controller = "BoardGame" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

using BoardGameApp.Data;
using BoardGameApp.Data.Models;
using BoardGameApp.Data.Repository.Interfaces;
using BoardGameApp.Data.Repository;
using BoardGameApp.Data.Seeding.Utilities;
using BoardGameApp.Services.Core;
using BoardGameApp.Services.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BoardGameApp.Web.Infrastructure.Middlewares;
using BoardGameApp.Data.Seeding;
using static BoardGameApp.GCommon.ApplicationConstants;
using BoardGameApp.GCommon;
using BoardGameApp.Services.Core.Admin.Interfaces;
using BoardGameApp.Services.Core.Admin;

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

builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IClubRepository, ClubRepository>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClubManagementService, ClubManagementService>();
builder.Services.AddScoped<IBoardGameManagementService, BoardGameManagementService>();

builder.Services.AddScoped<IBoardGameService, BoardGameService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IGameSessionService, GameSessionService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ITicketService, TicketService>();

WebApplication app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    RoleSeeder.SeedRoles(services);
    RoleSeeder.AssignAdminRole(services);
}


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

app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.Use((context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true && context.Request.Path == "/")
    {
        if (context.User.IsInRole(ApplicationConstants.RoleAdmin))
        {
            context.Response.Redirect("/Admin/Home/Index");
            return Task.CompletedTask;
        }
    }
    return next();
});
app.UseMiddleware<ManagerAccessMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "tickets",
    pattern: "Tickets/{action=Index}/{id?}",
    defaults: new { controller = "Ticket" });
app.MapControllerRoute(
    name: "reservations",
    pattern: "Reservations/{action=Index}/{id?}",
    defaults: new { controller = "Reservation" });
app.MapControllerRoute(
    name: "gameSessions",
    pattern: "GameSessions/{action=Index}/{id?}",
    defaults: new { controller = "GameSession" });
app.MapControllerRoute(
    name: "clubs",
    pattern: "Clubs/{action=Index}/{id?}",
    defaults: new { controller = "Club" });
app.MapControllerRoute(
    name: "boardgames",
    pattern: "BoardGames/{action=Index}/{id?}",
    defaults: new { controller = "BoardGame" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

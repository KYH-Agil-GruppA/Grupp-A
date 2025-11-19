using DAL.DbContext;
using DAL.Entities;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using Service.Services;
using Services.Interfaces;

namespace MainApp;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                      .AddRoles<IdentityRole<int>>()
                      .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddRazorPages();

        builder.Services.AddTransient<DataInitializer>();

        builder.Services.AddScoped<ISessionRepository, SessionRepository>();
        builder.Services.AddScoped<ISessionService, SessionService>();
        builder.Services.AddScoped<IBookingRepository, BookingRepository>();
        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IUserService, UserService>();


        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var inilizer = scope.ServiceProvider.GetRequiredService<DataInitializer>();
            await inilizer.SeedData();
        }
        app.UseStaticFiles();
        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        app.Run();
    }
}

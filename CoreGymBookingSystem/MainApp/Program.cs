using DAL.DbContext;
using DAL.Entities;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Services;
using Services.Interfaces;

namespace MainApp
{
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

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddRequestLocalization(options =>
            {
                options.SetDefaultCulture("en-US");
                options.AddSupportedCultures("en-US", "sv-SE");
                options.AddSupportedUICultures("en-US", "sv-SE");
            });

            builder.Services.AddTransient<DataInitializer>();


            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

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
}

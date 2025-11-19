using Microsoft.AspNetCore.Identity;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.DbContext;

/// <summary>
/// Seeds initial data for the application.
/// </summary>
/// <param name="dbContext">The database to seed for.</param>
/// <param name="userManager">The manager to seed users.</param>
public class DataInitializer( ApplicationDbContext dbContext, UserManager<User> userManager )
{
    /// <summary>
    /// Seeds initial data asynchronously.
    /// </summary>
    public async Task SeedData()
    {
        await dbContext.Database.MigrateAsync();

        await SeedRoles();
        await SeedUsers();
        await SeedSessions();

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds initial users for the application.
    /// </summary>
    private async Task SeedUsers()
    {
        await AddUserIfNotExists( "GruppA@gmail.com","Erik","Lund","Lundgatan 2", "Stockholm", "Sweden","Hejsan123#", ["Admin"] );
        await AddUserIfNotExists( "GruppA2@gmail.com","Amanda","Persson","Lundgatan 29","Stockholm","Sweden", "Hejsan123#", ["Member"] );
        await AddUserIfNotExists( "GruppA3@gmail.com","Gunila", "Andersson" ,"Lundgatan 3","Stockholm","Sweden","Hejsan123#", ["Trainer"] );
    }

    /// <summary>
    /// Seeds initial roles for the application.
    /// </summary>
    private async Task SeedRoles()
    {
        await AddRoleIfNotExisting( "Admin" );
        await AddRoleIfNotExisting( "Member" );
        await AddRoleIfNotExisting( "Trainer" );
    }

    /// <summary>
    /// Seeds initial sessions for the application.
    /// </summary>
    /// <returns></returns>
    private async Task SeedSessions()
    {
        if( dbContext.Sessions.Any() )
        {
            return;
        }

        var trainer = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "GruppA3@gmail.com");

        if( trainer == null )
        {
            return;
        }

        await dbContext.Sessions.AddRangeAsync(
            new Session
            {
                Title = "Morning Yoga",
                Description = "Start your day with calm yoga.",
                Category = "Yoga",
                Instructor = trainer,
                StartTime = DateTime.Today.AddHours( 6 ),
                EndTime = DateTime.Today.AddHours( 8 ),
                MaxParticipants = 15,
            },
            new Session
            {
                Title = "Cardio Blast",
                Description = "High-energy cardio to burn fat fast!",
                Category = "Cardio",
                Instructor = trainer,
                StartTime = DateTime.Today.AddHours( 10 ),
                EndTime = DateTime.Today.AddHours( 12 ),
                MaxParticipants = 20,
            },
            new Session
            {
                Title = "Evening Strength",
                Description = "Weightlifting and resistance training.",
                Category = "Strength",
                Instructor = trainer,
                StartTime = DateTime.Today.AddHours( 17 ),
                EndTime = DateTime.Today.AddHours( 19 ),
                MaxParticipants = 10,
            }
        );
    }

    /// <summary>
    /// Adds a role by name if it does not already exist.
    /// </summary>
    /// <param name="roleName">The name to add.</param>
    private async Task AddRoleIfNotExisting( string roleName )
    {
        var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        if( role != null )
        {
            return;
        }

        role = new IdentityRole<int> { Name = roleName, NormalizedName = roleName };
        await dbContext.Roles.AddAsync( role );
    }

    /// <summary>
    /// Adds a user by username if it doesn't already exist, along with password and roles.
    /// </summary>
    /// <param name="userName">The username to set. Is be used to identify the user.</param>
    /// <param name="password">The password to set. Will be hashed.</param>
    /// <param name="roles">Roles to give this user.</param>
    private async Task AddUserIfNotExists( string userName, string firstName, string lastName, string address, string city, string country , string password, string[] roles )
    {
        if( userManager.FindByEmailAsync( userName ).Result != null )
        {
            return;
        }

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Address = address,
            City = city,
            Country = country,
            UserName = userName,
            Email = userName,
            EmailConfirmed = true
        };

        await userManager.CreateAsync( user, password );
        await userManager.AddToRolesAsync( user, roles );
    }
}

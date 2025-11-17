using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DAL.DbContext;

/// <summary>
/// Wrapper for the application's database.
/// </summary>
/// <param name="options">The options to be used by a <see cref="ApplicationDbContext"/>.</param>
public partial class ApplicationDbContext( DbContextOptions<ApplicationDbContext> options ) : IdentityDbContext<User, IdentityRole<int>, int>( options )
{
    /// <summary>
    /// Set of all tracked sessions.
    /// </summary>
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<MembershipType> MembershipTypes { get; set; }



    /// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext.OnModelCreating"/>
    protected override void OnModelCreating( ModelBuilder builder )
    {
        builder.Entity<Session>().HasOne( e => e.Instructor );
        builder.Entity<Session>()

            .HasMany( e => e.Bookings )
            .WithMany( e => e.Bookings );

        base.OnModelCreating( builder );
        builder.Entity<MembershipType>()
               .HasData(SeedMembershipTypes());
    }
    private static MembershipType[] SeedMembershipTypes()
    {
        return new[]
        {
            new MembershipType
            {
                Id = 1,
                Name = "Adult Membership",
                Price = 399,
                Description = "Unlimited access to all classes and gym facilities.",
                ImageUrl = "/Gym_Tem/img/memberships/adult.jpg"

            },
            new MembershipType
            {
                Id = 2,
                Name = "Student Membership",
                Price = 299,
                Description = "Discounted membership for students with valid ID.",
                ImageUrl = "/Gym_Tem/img/memberships/student.jpg"

            },
            new MembershipType
            {
                Id = 3,
                Name = "Senior Membership",
                Price = 249,
                Description = "Full gym access with flexible hours for seniors aged 65+.",
                ImageUrl = "/Gym_Tem/img/memberships/senior.jpg"

            }
        };
    }
}

using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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



    /// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext.OnModelCreating"/>
    protected override void OnModelCreating( ModelBuilder builder )
    {
        builder.Entity<Session>().HasOne( e => e.Instructor );
        builder.Entity<Session>()

            .HasMany( e => e.Bookings )
            .WithMany( e => e.Bookings );

        base.OnModelCreating( builder );
    }
}

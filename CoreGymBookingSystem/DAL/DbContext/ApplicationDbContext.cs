using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.DbContext;

/// <summary>
/// Wrapper for the application's database.
/// </summary>
/// <param name="options">The options to be used by a <see cref="ApplicationDbContext"/>.</param>
public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    /// <summary>
    /// Set of all tracked sessions.
    /// </summary>
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<MembershipType> MembershipTypes { get; set; } = null!;

    /// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext.OnModelCreating"/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ============================================
        // SESSION ENTITY CONFIGURATION
        // ============================================
        builder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Session -> Instructor (Many-to-One)
            // One instructor can teach many sessions
            entity.HasOne(s => s.Instructor)
                .WithMany(u => u.InstructedSessions)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);
            // If instructor is deleted → set InstructorId = null

            // Session -> Bookings (One-to-Many)
            // One session can have many bookings
            entity.HasMany(s => s.Bookings)
                .WithOne(b => b.Session)
                .HasForeignKey(b => b.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            // If session is deleted → delete all its bookings

            entity.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(s => s.Description)
                .HasMaxLength(1000);

            entity.Property(s => s.Category)
                .HasMaxLength(50);

            // Indexes for performance
            entity.HasIndex(s => s.InstructorId);
            entity.HasIndex(s => s.Category);
        });

        // ============================================
        // BOOKING ENTITY CONFIGURATION
        // ============================================
        builder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Booking -> Session (Many-to-One)
            entity.HasOne(b => b.Session)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            // If session is deleted → delete this booking

            // Booking -> User (Many-to-One)
            entity.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // If user is deleted → delete this booking

            entity.Property(b => b.BookingDate)
                .HasDefaultValueSql("GETUTCDATE()");
            // Automatic UTC timestamp on insert

            entity.Property(b => b.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Confirmed");

            // CRITICAL: Prevent duplicate bookings
            // A user can only book a session ONCE
            entity.HasIndex(b => new { b.UserId, b.SessionId })
                .IsUnique()
                .HasDatabaseName("UX_Bookings_UserId_SessionId");

            // Performance indexes
            entity.HasIndex(b => b.UserId);
            entity.HasIndex(b => b.SessionId);
            entity.HasIndex(b => b.BookingDate);
        });

        // ============================================
        // USER ENTITY CONFIGURATION (optional but explicit)
        // ============================================
        builder.Entity<User>(entity =>
        {
            // User -> Bookings (One-to-Many)
            entity.HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // If user is deleted → delete all their bookings

            // User -> InstructedSessions (One-to-Many)
            entity.HasMany(u => u.InstructedSessions)
                .WithOne(s => s.Instructor)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);
            // If instructor is deleted → sessions get null instructor
        });

        // ============================================
        // NOTIFICATION ENTITY CONFIGURATION
        // ============================================
        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(n => n.Type)
                .HasMaxLength(50);

            // Index for performance
            entity.HasIndex(n => n.CreatedAt);
        });

        // ============================================
        // MEMBERSHIP TYPE CONFIGURATION
        // ============================================
        builder.Entity<MembershipType>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(m => m.Name)
                .IsRequired();

            entity.Property(m => m.Price)
                .HasPrecision(18, 2);

            // Seed initial data
            entity.HasData(
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
            );
        });
    }
}
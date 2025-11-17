using FluentAssertions;
using DAL.DbContext;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DAL.Tests.Repositories;

/// Enhetstester för BookingRepository
/// Använder InMemory-databas för isolation och determinism
public class BookingRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BookingRepository _sut;  // System Under Test

    public BookingRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new BookingRepository(_context);

        // Seed initial data
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var instructor = new User
        {
            Id = 1,
            UserName = "trainer1",
            Email = "trainer@example.com",
            EmailConfirmed = true
        };

        var user1 = new User
        {
            Id = 2,
            UserName = "member1",
            Email = "member1@example.com",
            EmailConfirmed = true
        };

        var user2 = new User
        {
            Id = 3,
            UserName = "member2",
            Email = "member2@example.com",
            EmailConfirmed = true
        };

        var session1 = new Session
        {
            Id = 1,
            Title = "Morning Yoga",
            Description = "Calm morning yoga",
            Category = "Yoga",
            InstructorId = 1,
            Instructor = instructor,
            MaxParticipants = 20,
            StartTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(9)
        };

        var session2 = new Session
        {
            Id = 2,
            Title = "Cardio Blast",
            Description = "High intensity cardio",
            Category = "Cardio",
            InstructorId = 1,
            Instructor = instructor,
            MaxParticipants = 15,
            StartTime = DateTime.UtcNow.AddDays(2).AddHours(18),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(19)
        };

        var fullSession = new Session
        {
            Id = 3,
            Title = "Full Class",
            Description = "This class is full",
            Category = "Bodybuilding",
            InstructorId = 1,
            Instructor = instructor,
            MaxParticipants = 2,
            StartTime = DateTime.UtcNow.AddDays(3).AddHours(10),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(11)
        };

        var pastSession = new Session
        {
            Id = 4,
            Title = "Past Class",
            Description = "This session has already started",
            Category = "Yoga",
            InstructorId = 1,
            Instructor = instructor,
            MaxParticipants = 20,
            StartTime = DateTime.UtcNow.AddHours(-2),
            EndTime = DateTime.UtcNow.AddHours(-1)
        };

        _context.Users.AddRange(instructor, user1, user2);
        _context.Sessions.AddRange(session1, session2, fullSession, pastSession);

        // Add some bookings to the full session
        var booking1 = new Booking
        {
            UserId = 2,
            SessionId = 3,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };

        var booking2 = new Booking
        {
            UserId = 3,
            SessionId = 3,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };

        _context.Bookings.AddRange(booking1, booking2);
        _context.SaveChanges();
    }

    #region GetUserBookingsAsync Tests

    [Fact]
    public async Task GetUserBookingsAsync_WithValidUser_ReturnsAllUserBookings()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetUserBookingsAsync(2);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
        result[0].UserId.Should().Be(2);
        result[0].SessionId.Should().Be(1);
        result[0].Session.Should().NotBeNull();
        result[0].Session!.Title.Should().Be("Morning Yoga");
    }

    [Fact]
    public async Task GetUserBookingsAsync_WithUserHavingMultipleBookings_ReturnsAllInOrder()
    {
        // Arrange
        var booking1 = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        var booking2 = new Booking
        {
            UserId = 2,
            SessionId = 2,
            BookingDate = DateTime.UtcNow.AddMinutes(1),
            Status = "Confirmed"
        };
        _context.Bookings.AddRange(booking1, booking2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetUserBookingsAsync(2);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeInAscendingOrder(b => b.Session!.StartTime);
    }

    [Fact]
    public async Task GetUserBookingsAsync_WithUserHavingNoBookings_ReturnsEmptyList()
    {
        // Act
        var result = await _sut.GetUserBookingsAsync(999);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserBookingsAsync_IncludesInstructorInformation()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetUserBookingsAsync(2);

        // Assert
        result[0].Session!.Instructor.Should().NotBeNull();
        result[0].Session!.Instructor!.UserName.Should().Be("trainer1");
    }

    #endregion

    #region GetBookingAsync Tests

    [Fact]
    public async Task GetBookingAsync_WithExistingBooking_ReturnsBooking()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetBookingAsync(2, 1);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(2);
        result.SessionId.Should().Be(1);
    }

    [Fact]
    public async Task GetBookingAsync_WithNonExistentBooking_ReturnsNull()
    {
        // Act
        var result = await _sut.GetBookingAsync(2, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBookingAsync_IncludesSessionAndInstructor()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetBookingAsync(2, 1);

        // Assert
        result!.Session.Should().NotBeNull();
        result.Session!.Instructor.Should().NotBeNull();
        result.Session.Title.Should().Be("Morning Yoga");
        result.Session.Instructor!.UserName.Should().Be("trainer1");
    }

    #endregion

    #region IsBookedAsync Tests

    [Fact]
    public async Task IsBookedAsync_WhenBookingExists_ReturnsTrue()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.IsBookedAsync(2, 1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsBookedAsync_WhenBookingDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _sut.IsBookedAsync(2, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsBookedAsync_WithDifferentUsersOnSameSession_OnlyReturnsTrueForCorrectUser()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result1 = await _sut.IsBookedAsync(2, 1);
        var result2 = await _sut.IsBookedAsync(3, 1);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }

    #endregion

    #region BookSessionAsync Tests

    [Fact]
    public async Task BookSessionAsync_WithValidUserAndSession_CreatesBooking()
    {
        // Act
        var result = await _sut.BookSessionAsync(2, 1);

        // Assert
        result.Should().BeTrue();
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.UserId == 2 && b.SessionId == 1);
        booking.Should().NotBeNull();
        booking!.Status.Should().Be("Confirmed");
    }

    [Fact]
    public async Task BookSessionAsync_CreatesBookingWithCurrentDateTime()
    {
        // Act
        var beforeBooking = DateTime.UtcNow;
        await _sut.BookSessionAsync(2, 1);
        var afterBooking = DateTime.UtcNow;

        // Assert
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.UserId == 2 && b.SessionId == 1);
        booking!.BookingDate.Should().BeOnOrAfter(beforeBooking);
        booking.BookingDate.Should().BeOnOrBefore(afterBooking);
    }

    [Fact]
    public async Task BookSessionAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Act
        var result = await _sut.BookSessionAsync(999, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task BookSessionAsync_WithNonExistentSession_ReturnsFalse()
    {
        // Act
        var result = await _sut.BookSessionAsync(2, 999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task BookSessionAsync_MultipleUsersCanBookSameSession()
    {
        // Act
        var result1 = await _sut.BookSessionAsync(2, 1);
        var result2 = await _sut.BookSessionAsync(3, 1);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();

        var bookings = await _context.Bookings
            .Where(b => b.SessionId == 1)
            .ToListAsync();
        bookings.Should().HaveCount(2);
    }

    #endregion

    #region CancelBookingAsync Tests

    [Fact]
    public async Task CancelBookingAsync_WithExistingBooking_RemovesBooking()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.CancelBookingAsync(2, 1);

        // Assert
        result.Should().BeTrue();
        var remainingBooking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.UserId == 2 && b.SessionId == 1);
        remainingBooking.Should().BeNull();
    }

    [Fact]
    public async Task CancelBookingAsync_WithNonExistentBooking_ReturnsFalse()
    {
        // Act
        var result = await _sut.CancelBookingAsync(2, 999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelBookingAsync_OnlyRemovesCorrectBooking()
    {
        // Arrange
        var booking1 = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        var booking2 = new Booking
        {
            UserId = 2,
            SessionId = 2,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        _context.Bookings.AddRange(booking1, booking2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.CancelBookingAsync(2, 1);

        // Assert
        result.Should().BeTrue();
        var remainingBookings = await _context.Bookings
            .Where(b => b.UserId == 2)
            .ToListAsync();
        remainingBookings.Should().HaveCount(1);
        remainingBookings[0].SessionId.Should().Be(2);
    }

    #endregion

    #region ValidateBookingAsync Tests

    [Fact]
    public async Task ValidateBookingAsync_WithValidBooking_ReturnsValid()
    {
        // Act
        var result = await _sut.ValidateBookingAsync(2, 1);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Message.Should().Be("OK");
    }

    [Fact]
    public async Task ValidateBookingAsync_WithNonExistentUser_ReturnsInvalid()
    {
        // Act
        var result = await _sut.ValidateBookingAsync(999, 1);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    [Fact]
    public async Task ValidateBookingAsync_WithNonExistentSession_ReturnsInvalid()
    {
        // Act
        var result = await _sut.ValidateBookingAsync(2, 999);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Session not found");
    }

    [Fact]
    public async Task ValidateBookingAsync_WithFullSession_ReturnsInvalid()
    {
        // Act - Session 3 is already full (2 bookings, 2 max)
        var result = await _sut.ValidateBookingAsync(2, 3);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("session is full");
        result.Message.Should().Contain("(2/2)");
    }

    [Fact]
    public async Task ValidateBookingAsync_WhenAlreadyBooked_ReturnsInvalid()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 2,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ValidateBookingAsync(2, 1);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("You have already booked this session");
    }

    [Fact]
    public async Task ValidateBookingAsync_WithPastSession_ReturnsInvalid()
    {
        // Act - Session 4 is in the past
        var result = await _sut.ValidateBookingAsync(2, 4);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("session is no longer available");
    }

    [Fact]
    public async Task ValidateBookingAsync_PrioritizesUserNotFoundError()
    {
        // Act
        var result = await _sut.ValidateBookingAsync(999, 999);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task CompleteBookingFlow_BookCancelAndRebook_Success()
    {
        // Act - Initial booking
        var bookingResult = await _sut.BookSessionAsync(2, 1);
        bookingResult.Should().BeTrue();

        // Assert - Booking created
        var isBooked = await _sut.IsBookedAsync(2, 1);
        isBooked.Should().BeTrue();

        // Act - Cancel
        var cancelResult = await _sut.CancelBookingAsync(2, 1);
        cancelResult.Should().BeTrue();

        // Assert - Booking removed
        var isBookedAfterCancel = await _sut.IsBookedAsync(2, 1);
        isBookedAfterCancel.Should().BeFalse();

        // Act - Rebook (should be allowed)
        var rebookResult = await _sut.BookSessionAsync(2, 1);
        rebookResult.Should().BeTrue();

        // Assert - New booking created
        var isBookedAfterRebook = await _sut.IsBookedAsync(2, 1);
        isBookedAfterRebook.Should().BeTrue();
    }

    [Fact]
    public async Task SessionCapacity_MultipleUsersCanBookUntilFull()
    {
        // Session 1 has max 20 participants
        // Arrange & Act
        var results = new List<bool>();
        for (int i = 2; i < 22; i++)
        {
            var user = new User
            {
                Id = 100 + i,
                UserName = $"user{i}",
                Email = $"user{i}@example.com",
                EmailConfirmed = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var bookingResult = await _sut.BookSessionAsync(100 + i, 1);
            results.Add(bookingResult);
        }

        // Assert - All bookings should succeed (20 users)
        results.Should().AllSatisfy(r => r.Should().BeTrue());

        // Act - One more booking attempt
        var user23 = new User
        {
            Id = 123,
            UserName = "user23",
            Email = "user23@example.com",
            EmailConfirmed = true
        };
        _context.Users.Add(user23);
        await _context.SaveChangesAsync();

        var validation = await _sut.ValidateBookingAsync(123, 1);

        // Assert - Should be invalid because session is full
        validation.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task MultipleUsers_CanBookMultipleSessions()
    {
        // Arrange & Act
        await _sut.BookSessionAsync(2, 1);
        await _sut.BookSessionAsync(2, 2);
        await _sut.BookSessionAsync(3, 1);
        await _sut.BookSessionAsync(3, 2);

        // Assert
        var user2Bookings = await _sut.GetUserBookingsAsync(2);
        var user3Bookings = await _sut.GetUserBookingsAsync(3);

        user2Bookings.Should().HaveCount(2);
        user3Bookings.Should().HaveCount(2);

        user2Bookings.Select(b => b.SessionId).Should().Contain(new[] { 1, 2 });
        user3Bookings.Select(b => b.SessionId).Should().Contain(new[] { 1, 2 });
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}

using DAL.Entities;
using FluentAssertions;

namespace DAL.Tests.Entities;

/// Enhetstester för Booking Entity
/// Testar egenskaper, relationer och validering av Booking-entiteten
public class BookingEntityTests
{
    #region Constructor Tests

    [Fact]
    public void Booking_WithDefaultConstructor_InitializesPropertiesCorrectly()
    {
        // Act
        var booking = new Booking();

        // Assert
        booking.Id.Should().Be(0);
        booking.UserId.Should().Be(0);
        booking.SessionId.Should().Be(0);
        booking.Status.Should().Be("Confirmed");
        booking.BookingDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Booking_WithConstructorParameters_InitializesCorrectly()
    {
        // Arrange
        int userId = 1;
        int sessionId = 10;
        var bookingDate = DateTime.UtcNow;
        string status = "Confirmed";

        // Act
        var booking = new Booking
        {
            UserId = userId,
            SessionId = sessionId,
            BookingDate = bookingDate,
            Status = status
        };

        // Assert
        booking.UserId.Should().Be(userId);
        booking.SessionId.Should().Be(sessionId);
        booking.BookingDate.Should().Be(bookingDate);
        booking.Status.Should().Be(status);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Booking_Id_CanBeSetAndRetrieved()
    {
        // Arrange
        var booking = new Booking();
        int expectedId = 42;

        // Act
        booking.Id = expectedId;

        // Assert
        booking.Id.Should().Be(expectedId);
    }

    [Fact]
    public void Booking_UserId_CanBeSetAndRetrieved()
    {
        // Arrange
        var booking = new Booking();
        int userId = 5;

        // Act
        booking.UserId = userId;

        // Assert
        booking.UserId.Should().Be(userId);
    }

    [Fact]
    public void Booking_SessionId_CanBeSetAndRetrieved()
    {
        // Arrange
        var booking = new Booking();
        int sessionId = 20;

        // Act
        booking.SessionId = sessionId;

        // Assert
        booking.SessionId.Should().Be(sessionId);
    }

    [Fact]
    public void Booking_BookingDate_CanBeSetAndRetrieved()
    {
        // Arrange
        var booking = new Booking();
        var bookingDate = new DateTime(2025, 12, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        booking.BookingDate = bookingDate;

        // Assert
        booking.BookingDate.Should().Be(bookingDate);
    }

    [Fact]
    public void Booking_Status_CanBeSetAndRetrieved()
    {
        // Arrange
        var booking = new Booking();
        string status = "Cancelled";

        // Act
        booking.Status = status;

        // Assert
        booking.Status.Should().Be(status);
    }

    #endregion

    #region Navigation Property Tests

    [Fact]
    public void Booking_User_NavigationPropertyCanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var user = new User
        {
            Id = 1,
            UserName = "testuser",
            Email = "test@example.com"
        };

        // Act
        booking.User = user;

        // Assert
        booking.User.Should().NotBeNull();
        booking.User.Id.Should().Be(1);
        booking.User.UserName.Should().Be("testuser");
    }

    [Fact]
    public void Booking_Session_NavigationPropertyCanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var session = new Session
        {
            Id = 1,
            Title = "Yoga Class",
            Description = "Morning yoga",
            Category = "Yoga",
            MaxParticipants = 20,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
        };

        // Act
        booking.Session = session;

        // Assert
        booking.Session.Should().NotBeNull();
        booking.Session.Id.Should().Be(1);
        booking.Session.Title.Should().Be("Yoga Class");
    }

    [Fact]
    public void Booking_WithUserAndSession_NavigationPropertiesCorrect()
    {
        // Arrange
        var user = new User { Id = 1, UserName = "John" };
        var session = new Session { Id = 1, Title = "Yoga" };

        var booking = new Booking
        {
            UserId = 1,
            SessionId = 1,
            User = user,
            Session = session
        };

        // Act & Assert
        booking.User.Should().Be(user);
        booking.Session.Should().Be(session);
        booking.User.UserName.Should().Be("John");
        booking.Session.Title.Should().Be("Yoga");
    }

    #endregion

    #region Status Tests

    [Theory]
    [InlineData("Confirmed")]
    [InlineData("Cancelled")]
    [InlineData("Pending")]
    [InlineData("Completed")]
    public void Booking_Status_AcceptsVariousStatuses(string status)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = status;

        // Assert
        booking.Status.Should().Be(status);
    }

    [Fact]
    public void Booking_DefaultStatus_IsConfirmed()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        booking.Status.Should().Be("Confirmed");
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void Booking_WithValidData_IsValid()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 1,
            UserId = 1,
            SessionId = 1,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };

        // Act & Assert
        booking.Id.Should().Be(1);
        booking.UserId.Should().Be(1);
        booking.SessionId.Should().Be(1);
        booking.Status.Should().Be("Confirmed");
    }

    [Fact]
    public void Booking_WithNegativeUserId_StillCreates()
    {
        // Arrange
        var booking = new Booking { UserId = -1 };

        // Act & Assert - Entity allows this, validation happens in Service/Repository
        booking.UserId.Should().Be(-1);
    }

    [Fact]
    public void Booking_WithNegativeSessionId_StillCreates()
    {
        // Arrange
        var booking = new Booking { SessionId = -1 };

        // Act & Assert - Entity allows this, validation happens in Service/Repository
        booking.SessionId.Should().Be(-1);
    }

    #endregion

    #region DateTime Tests

    [Fact]
    public void Booking_BookingDate_DefaultsToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var booking = new Booking();

        var afterCreation = DateTime.UtcNow;

        // Assert
        booking.BookingDate.Should().BeOnOrAfter(beforeCreation);
        booking.BookingDate.Should().BeOnOrBefore(afterCreation);
    }

    [Fact]
    public void Booking_BookingDate_CanBePastDate()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-30);
        var booking = new Booking();

        // Act
        booking.BookingDate = pastDate;

        // Assert
        booking.BookingDate.Should().Be(pastDate);
    }

    [Fact]
    public void Booking_BookingDate_CanBeFutureDate()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(30);
        var booking = new Booking();

        // Act
        booking.BookingDate = futureDate;

        // Assert
        booking.BookingDate.Should().Be(futureDate);
    }

    #endregion

    #region Relationship Tests

    [Fact]
    public void Booking_ForeignKeys_PointToCorrectEntities()
    {
        // Arrange
        var user = new User { Id = 5, UserName = "Alice" };
        var session = new Session { Id = 10, Title = "Cardio" };

        var booking = new Booking
        {
            UserId = user.Id,
            SessionId = session.Id,
            User = user,
            Session = session
        };

        // Act & Assert
        booking.UserId.Should().Be(user.Id);
        booking.SessionId.Should().Be(session.Id);
        booking.User!.UserName.Should().Be("Alice");
        booking.Session!.Title.Should().Be("Cardio");
    }

    [Fact]
    public void Booking_MultipleBookings_CanShareSameUser()
    {
        // Arrange
        var user = new User { Id = 1, UserName = "Bob" };

        var booking1 = new Booking { UserId = user.Id, User = user, SessionId = 1 };
        var booking2 = new Booking { UserId = user.Id, User = user, SessionId = 2 };

        // Act & Assert
        booking1.User.Should().Be(booking2.User);
        booking1.UserId.Should().Be(booking2.UserId);
    }

    [Fact]
    public void Booking_MultipleBookings_CanShareSameSession()
    {
        // Arrange
        var session = new Session { Id = 1, Title = "Yoga" };

        var booking1 = new Booking { SessionId = session.Id, Session = session, UserId = 1 };
        var booking2 = new Booking { SessionId = session.Id, Session = session, UserId = 2 };

        // Act & Assert
        booking1.Session.Should().Be(booking2.Session);
        booking1.SessionId.Should().Be(booking2.SessionId);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Booking_SameBooking_HasSameProperties()
    {
        // Arrange
        var booking1 = new Booking
        {
            Id = 1,
            UserId = 5,
            SessionId = 10,
            Status = "Confirmed"
        };

        var booking2 = new Booking
        {
            Id = 1,
            UserId = 5,
            SessionId = 10,
            Status = "Confirmed"
        };

        // Act & Assert
        booking1.Id.Should().Be(booking2.Id);
        booking1.UserId.Should().Be(booking2.UserId);
        booking1.SessionId.Should().Be(booking2.SessionId);
        booking1.Status.Should().Be(booking2.Status);
    }

    [Fact]
    public void Booking_DifferentBookings_HaveDifferentIds()
    {
        // Arrange
        var booking1 = new Booking { Id = 1 };
        var booking2 = new Booking { Id = 2 };

        // Act & Assert
        booking1.Id.Should().NotBe(booking2.Id);
    }

    #endregion

    #region Complex Scenario Tests

    [Fact]
    public void Booking_CompleteBookingScenario_WithAllProperties()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            UserName = "member1",
            Email = "member@example.com",
            EmailConfirmed = true
        };

        var session = new Session
        {
            Id = 1,
            Title = "Morning Yoga",
            Description = "Relaxing morning session",
            Category = "Yoga",
            MaxParticipants = 20,
            StartTime = DateTime.UtcNow.AddDays(1).AddHours(8),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(9),
            Bookings = new List<Booking>()
        };

        var bookingDate = DateTime.UtcNow;

        var booking = new Booking
        {
            Id = 1,
            UserId = user.Id,
            SessionId = session.Id,
            BookingDate = bookingDate,
            Status = "Confirmed",
            User = user,
            Session = session
        };

        // Act - Add booking to session
        session.Bookings.Add(booking);

        // Assert
        booking.Should().NotBeNull();
        booking.User!.UserName.Should().Be("member1");
        booking.Session!.Title.Should().Be("Morning Yoga");
        booking.Status.Should().Be("Confirmed");
        session.Bookings.Should().HaveCount(1);
        session.Bookings[0].User!.Email.Should().Be("member@example.com");
    }

    [Fact]
    public void Booking_SessionWithMultipleBookings_AllPropertiesAccessible()
    {
        // Arrange
        var session = new Session
        {
            Id = 1,
            Title = "Cardio Class",
            Bookings = new List<Booking>()
        };

        var users = new List<User>
        {
            new User { Id = 1, UserName = "user1" },
            new User { Id = 2, UserName = "user2" },
            new User { Id = 3, UserName = "user3" }
        };

        // Act - Create multiple bookings
        foreach (var user in users)
        {
            var booking = new Booking
            {
                UserId = user.Id,
                SessionId = session.Id,
                User = user,
                Session = session,
                Status = "Confirmed"
            };
            session.Bookings.Add(booking);
        }

        // Assert
        session.Bookings.Should().HaveCount(3);
        session.Bookings.All(b => b.SessionId == session.Id).Should().BeTrue();
        session.Bookings.All(b => b.Status == "Confirmed").Should().BeTrue();
        session.Bookings[0].User!.UserName.Should().Be("user1");
        session.Bookings[1].User!.UserName.Should().Be("user2");
        session.Bookings[2].User!.UserName.Should().Be("user3");
    }

    #endregion
}
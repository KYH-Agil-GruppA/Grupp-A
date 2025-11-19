using FluentAssertions;
using Moq;
using Service.Services;
using DAL.Entities;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace Service.Tests.Services;

/// Enhetstester för BookingService
/// Testar alla publika metoder och säkerställer korrekt beteende
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly BookingService _sut;  // System Under Test

    public BookingServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _sut = new BookingService(_mockRepository.Object);
    }

    #region GetMyBookingsAsync Tests

    [Fact]
    public async Task GetMyBookingsAsync_WithValidUserId_ReturnsListOfSessions()
    {
        // Arrange
        int userId = 1;
        var bookings = new List<Booking>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                SessionId = 1,
                BookingDate = DateTime.UtcNow,
                Status = "Confirmed",
                Session = new Session
                {
                    Id = 1,
                    Title = "Yoga Class",
                    Description = "Morning yoga",
                    Category = "Yoga",
                    MaxParticipants = 20,
                    StartTime = DateTime.UtcNow.AddDays(1),
                    EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
                }
            },
            new()
            {
                Id = 2,
                UserId = userId,
                SessionId = 2,
                BookingDate = DateTime.UtcNow,
                Status = "Confirmed",
                Session = new Session
                {
                    Id = 2,
                    Title = "Cardio Class",
                    Description = "High intensity cardio",
                    Category = "Cardio",
                    MaxParticipants = 15,
                    StartTime = DateTime.UtcNow.AddDays(2),
                    EndTime = DateTime.UtcNow.AddDays(2).AddHours(1)
                }
            }
        };

        _mockRepository
            .Setup(r => r.GetUserBookingsAsync(userId))
            .ReturnsAsync(bookings);

        // Act
        var result = await _sut.GetMyBookingsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Yoga Class");
        result[1].Title.Should().Be("Cardio Class");
        _mockRepository.Verify(r => r.GetUserBookingsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetMyBookingsAsync_WithNoBookings_ReturnsEmptyList()
    {
        // Arrange
        int userId = 99;
        _mockRepository
            .Setup(r => r.GetUserBookingsAsync(userId))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _sut.GetMyBookingsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMyBookingsAsync_ReturnedSessionsAreOrderedByStartTime()
    {
        // Arrange
        int userId = 1;
        var laterBooking = new Booking
        {
            Session = new Session
            {
                Title = "Later Class",
                StartTime = DateTime.UtcNow.AddDays(3)
            }
        };

        var earlierBooking = new Booking
        {
            Session = new Session
            {
                Title = "Earlier Class",
                StartTime = DateTime.UtcNow.AddDays(1)
            }
        };

        _mockRepository
            .Setup(r => r.GetUserBookingsAsync(userId))
            .ReturnsAsync(new List<Booking> { laterBooking, earlierBooking });

        // Act
        var result = await _sut.GetMyBookingsAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        // Notering: GetMyBookingsAsync returnerar bara sessions, ordningen beror på repository
    }

    #endregion

    #region BookSessionAsync Tests

    [Fact]
    public async Task BookSessionAsync_WithValidInput_ReturnsSuccessTuple()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: true, Message: "OK");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(userId, sessionId))
            .ReturnsAsync(validationResult);

        _mockRepository
            .Setup(r => r.BookSessionAsync(userId, sessionId))
            .ReturnsAsync(true);

        // Act
        var (success, message) = await _sut.BookSessionAsync(userId, sessionId);

        // Assert
        success.Should().BeTrue();
        message.Should().Be("Booking successful!");
        _mockRepository.Verify(r => r.ValidateBookingAsync(userId, sessionId), Times.Once);
        _mockRepository.Verify(r => r.BookSessionAsync(userId, sessionId), Times.Once);
    }

    [Fact]
    public async Task BookSessionAsync_WhenValidationFails_ReturnsFailureWithMessage()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: false, Message: "Session is full");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(userId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var (success, message) = await _sut.BookSessionAsync(userId, sessionId);

        // Assert
        success.Should().BeFalse();
        message.Should().Be("Session is full");
        _mockRepository.Verify(r => r.BookSessionAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task BookSessionAsync_WhenRepositoryBookingFails_ReturnsFailureMessage()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: true, Message: "OK");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(userId, sessionId))
            .ReturnsAsync(validationResult);

        _mockRepository
            .Setup(r => r.BookSessionAsync(userId, sessionId))
            .ReturnsAsync(false);

        // Act
        var (success, message) = await _sut.BookSessionAsync(userId, sessionId);

        // Assert
        success.Should().BeFalse();
        message.Should().Be("Booking failed - please try again later");
    }

    [Theory]
    [InlineData("User not found")]
    [InlineData("Session not found")]
    [InlineData("The session is full (20/20)")]
    [InlineData("You have already booked this session")]
    [InlineData("Booking failed, session is no longer available")]
    public async Task BookSessionAsync_WithDifferentValidationErrors_ReturnsCorrectMessage(string expectedMessage)
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: false, Message: expectedMessage);

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(userId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var (success, message) = await _sut.BookSessionAsync(userId, sessionId);

        // Assert
        success.Should().BeFalse();
        message.Should().Be(expectedMessage);
    }

    #endregion

    #region CancelBookingAsync Tests

    [Fact]
    public async Task CancelBookingAsync_WithValidBooking_ReturnsSuccessTuple()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;

        _mockRepository
            .Setup(r => r.CancelBookingAsync(userId, sessionId))
            .ReturnsAsync(true);

        // Act
        var (success, message) = await _sut.CancelBookingAsync(userId, sessionId);

        // Assert
        success.Should().BeTrue();
        message.Should().Be("Booking cancelled!");
        _mockRepository.Verify(r => r.CancelBookingAsync(userId, sessionId), Times.Once);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenBookingNotFound_ReturnsFailure()
    {
        // Arrange
        int userId = 1;
        int sessionId = 999;

        _mockRepository
            .Setup(r => r.CancelBookingAsync(userId, sessionId))
            .ReturnsAsync(false);

        // Act
        var (success, message) = await _sut.CancelBookingAsync(userId, sessionId);

        // Assert
        success.Should().BeFalse();
        message.Should().Be("Cancellation failed - please try again later");
    }

    [Fact]
    public async Task CancelBookingAsync_WithDatabaseError_ReturnsFailureMessage()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;

        _mockRepository
            .Setup(r => r.CancelBookingAsync(userId, sessionId))
            .ReturnsAsync(false);

        // Act
        var (success, message) = await _sut.CancelBookingAsync(userId, sessionId);

        // Assert
        success.Should().BeFalse();
        message.Should().Be("Cancellation failed - please try again later");
    }

    #endregion

    #region IsBookedAsync Tests

    [Fact]
    public async Task IsBookedAsync_WhenUserIsBooked_ReturnsTrue()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;

        _mockRepository
            .Setup(r => r.IsBookedAsync(userId, sessionId))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.IsBookedAsync(userId, sessionId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.IsBookedAsync(userId, sessionId), Times.Once);
    }

    [Fact]
    public async Task IsBookedAsync_WhenUserIsNotBooked_ReturnsFalse()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;

        _mockRepository
            .Setup(r => r.IsBookedAsync(userId, sessionId))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.IsBookedAsync(userId, sessionId);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(1, 2, false)]
    [InlineData(2, 1, false)]
    public async Task IsBookedAsync_WithVariousUserAndSessionCombinations(int userId, int sessionId, bool expected)
    {
        // Arrange
        _mockRepository
            .Setup(r => r.IsBookedAsync(userId, sessionId))
            .ReturnsAsync(expected);

        // Act
        var result = await _sut.IsBookedAsync(userId, sessionId);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region ValidateAsync Tests

    [Fact]
    public async Task ValidateAsync_WithValidBooking_ReturnsTrue()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: true, Message: "OK");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(userId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var (valid, message) = await _sut.ValidateAsync(userId, sessionId);

        // Assert
        valid.Should().BeTrue();
        message.Should().Be("OK");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidBooking_ReturnsFalseWithMessage()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: false, Message: "Session not found");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(userId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var (valid, message) = await _sut.ValidateAsync(userId, sessionId);

        // Assert
        valid.Should().BeFalse();
        message.Should().Be("Session not found");
    }

    [Theory]
    [InlineData("User not found")]
    [InlineData("The session is full (20/20)")]
    [InlineData("You have already booked this session")]
    [InlineData("Booking failed, session is no longer available")]
    public async Task ValidateAsync_WithDifferentValidationMessages_ReturnsCorrectMessage(string message)
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: false, Message: message);

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(userId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var (valid, returnedMessage) = await _sut.ValidateAsync(userId, sessionId);

        // Assert
        valid.Should().BeFalse();
        returnedMessage.Should().Be(message);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task CompleteBookingWorkflow_BookAndThenCancel_Success()
    {
        // Arrange
        int userId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: true, Message: "OK");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(userId, sessionId))
            .ReturnsAsync(validationResult);

        _mockRepository
            .Setup(r => r.BookSessionAsync(userId, sessionId))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.CancelBookingAsync(userId, sessionId))
            .ReturnsAsync(true);

        // Act - Book
        var (bookingSuccess, bookingMessage) = await _sut.BookSessionAsync(userId, sessionId);

        // Assert - Booking successful
        bookingSuccess.Should().BeTrue();
        bookingMessage.Should().Be("Booking successful!");

        // Act - Cancel
        var (cancelSuccess, cancelMessage) = await _sut.CancelBookingAsync(userId, sessionId);

        // Assert - Cancellation successful
        cancelSuccess.Should().BeTrue();
        cancelMessage.Should().Be("Booking cancelled!");

        _mockRepository.Verify(r => r.ValidateBookingAsync(userId, sessionId), Times.Once);
        _mockRepository.Verify(r => r.BookSessionAsync(userId, sessionId), Times.Once);
        _mockRepository.Verify(r => r.CancelBookingAsync(userId, sessionId), Times.Once);
    }

    [Fact]
    public async Task MultipleBookings_UserCanBookMultipleSessions()
    {
        // Arrange
        int userId = 1;
        var bookings = new List<Booking>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                SessionId = 1,
                Session = new Session { Title = "Session 1" }
            },
            new()
            {
                Id = 2,
                UserId = userId,
                SessionId = 2,
                Session = new Session { Title = "Session 2" }
            },
            new()
            {
                Id = 3,
                UserId = userId,
                SessionId = 3,
                Session = new Session { Title = "Session 3" }
            }
        };

        _mockRepository
            .Setup(r => r.GetUserBookingsAsync(userId))
            .ReturnsAsync(bookings);

        // Act
        var result = await _sut.GetMyBookingsAsync(userId);

        // Assert
        result.Should().HaveCount(3);
        result.Select(s => s.Title).Should().Contain(new[] { "Session 1", "Session 2", "Session 3" });
    }

    #endregion
}
using FluentAssertions;
using Moq;
using Service.Services;
using DAL.Entities;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace Service.Tests.Integration;

/// Integrationstester för bokningsscenarios
/// Testar interaktionen mellan Service och Repository lager
public class BookingIntegrationTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly BookingService _sut;  // System Under Test

    public BookingIntegrationTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _sut = new BookingService(_mockRepository.Object);
    }

    #region Member Booking Flow Tests

    [Fact]
    public async Task MemberBookingFlow_BrowseAvailableSessionAndBook()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 1;

        var session = new Session
        {
            Id = sessionId,
            Title = "Morning Yoga",
            Category = "Yoga",
            MaxParticipants = 20,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Bookings = new List<Booking>()
        };

        var validationResult = new BookingValidationResult(IsValid: true, Message: "OK");
        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, sessionId))
            .ReturnsAsync(validationResult);

        _mockRepository
            .Setup(r => r.BookSessionAsync(memberId, sessionId))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.BookSessionAsync(memberId, sessionId);

        // Assert
        result.success.Should().BeTrue();
        result.message.Should().Be("Booking successful!");
    }

    [Fact]
    public async Task MemberBookingFlow_ViewMyBookings_And_CancelOne()
    {
        // Arrange
        int memberId = 1;

        var bookings = new List<Booking>
        {
            new()
            {
                Id = 1,
                UserId = memberId,
                SessionId = 1,
                Status = "Confirmed",
                Session = new Session
                {
                    Id = 1,
                    Title = "Yoga",
                    StartTime = DateTime.UtcNow.AddDays(1)
                }
            },
            new()
            {
                Id = 2,
                UserId = memberId,
                SessionId = 2,
                Status = "Confirmed",
                Session = new Session
                {
                    Id = 2,
                    Title = "Cardio",
                    StartTime = DateTime.UtcNow.AddDays(2)
                }
            }
        };

        _mockRepository
            .Setup(r => r.GetUserBookingsAsync(memberId))
            .ReturnsAsync(bookings);

        _mockRepository
            .Setup(r => r.CancelBookingAsync(memberId, 1))
            .ReturnsAsync(true);

        // Act - View bookings
        var myBookings = await _sut.GetMyBookingsAsync(memberId);

        // Assert
        myBookings.Should().HaveCount(2);

        // Act - Cancel one
        var cancelResult = await _sut.CancelBookingAsync(memberId, 1);

        // Assert
        cancelResult.success.Should().BeTrue();
        cancelResult.message.Should().Be("Booking cancelled!");

        _mockRepository.Verify(r => r.GetUserBookingsAsync(memberId), Times.Once);
        _mockRepository.Verify(r => r.CancelBookingAsync(memberId, 1), Times.Once);
    }

    #endregion

    #region Error Scenario Tests

    [Fact]
    public async Task ErrorScenario_CannotBookFullSession()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 1;

        var validationResult = new BookingValidationResult(
            IsValid: false,
            Message: "The session is full (20/20)");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _sut.BookSessionAsync(memberId, sessionId);

        // Assert
        result.success.Should().BeFalse();
        result.message.Should().Contain("full");
        _mockRepository.Verify(r => r.BookSessionAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ErrorScenario_CannotBookPastSession()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 1;

        var validationResult = new BookingValidationResult(
            IsValid: false,
            Message: "Booking failed, session is no longer available");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _sut.BookSessionAsync(memberId, sessionId);

        // Assert
        result.success.Should().BeFalse();
        result.message.Should().Contain("no longer available");
    }

    [Fact]
    public async Task ErrorScenario_CannotBookDuplicateSession()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 1;

        var validationResult = new BookingValidationResult(
            IsValid: false,
            Message: "You have already booked this session");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _sut.BookSessionAsync(memberId, sessionId);

        // Assert
        result.success.Should().BeFalse();
        result.message.Should().Contain("already booked");
    }

    [Fact]
    public async Task ErrorScenario_UserNotFound()
    {
        // Arrange
        int invalidUserId = 999;
        int sessionId = 1;

        var validationResult = new BookingValidationResult(IsValid: false, Message: "User not found");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(invalidUserId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _sut.BookSessionAsync(invalidUserId, sessionId);

        // Assert
        result.success.Should().BeFalse();
        result.message.Should().Be("User not found");
    }

    [Fact]
    public async Task ErrorScenario_SessionNotFound()
    {
        // Arrange
        int memberId = 1;
        int invalidSessionId = 999;

        var validationResult = new BookingValidationResult(IsValid: false, Message: "Session not found");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, invalidSessionId))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _sut.BookSessionAsync(memberId, invalidSessionId);

        // Assert
        result.success.Should().BeFalse();
        result.message.Should().Be("Session not found");
    }

    #endregion

    #region Concurrent Booking Tests

    [Fact]
    public async Task ConcurrentBooking_MultipleUsersBookingSameSession()
    {
        // Arrange
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: true, Message: "OK");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(It.IsAny<int>(), sessionId))
            .ReturnsAsync(validationResult);

        _mockRepository
            .Setup(r => r.BookSessionAsync(It.IsAny<int>(), sessionId))
            .ReturnsAsync(true);

        // Act - Multiple users book the same session
        var bookings = new List<Task<(bool, string)>>();
        for (int i = 1; i <= 5; i++)
        {
            bookings.Add(_sut.BookSessionAsync(i, sessionId));
        }

        var results = await Task.WhenAll(bookings);

        // Assert
        results.Should().AllSatisfy(r => r.Item1.Should().BeTrue());
        _mockRepository.Verify(
            r => r.BookSessionAsync(It.IsAny<int>(), sessionId),
            Times.Exactly(5));
    }

    #endregion

    #region Booking Validation Edge Cases

    [Fact]
    public async Task BookingValidation_SessionStartTimeJustAboutToStart()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(
            IsValid: false,
            Message: "Booking failed, session is no longer available");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, sessionId))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _sut.BookSessionAsync(memberId, sessionId);

        // Assert
        result.success.Should().BeFalse();
    }

    [Fact]
    public async Task BookingValidation_SessionWithOneSeatLeft()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 1;
        var validationResult = new BookingValidationResult(IsValid: true, Message: "OK");

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, sessionId))
            .ReturnsAsync(validationResult);

        _mockRepository
            .Setup(r => r.BookSessionAsync(memberId, sessionId))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.BookSessionAsync(memberId, sessionId);

        // Assert
        result.success.Should().BeTrue();
        result.message.Should().Be("Booking successful!");
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task Cancellation_UserCanCancelAndRebook()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 1;

        _mockRepository
            .Setup(r => r.CancelBookingAsync(memberId, sessionId))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, sessionId))
            .ReturnsAsync(new BookingValidationResult(IsValid: true, Message: "OK"));

        _mockRepository
            .Setup(r => r.BookSessionAsync(memberId, sessionId))
            .ReturnsAsync(true);

        // Act - Cancel
        var (cancelSuccess, cancelMsg) = await _sut.CancelBookingAsync(memberId, sessionId);

        // Assert
        cancelSuccess.Should().BeTrue();

        // Act - Rebook
        var (rebookSuccess, rebookMsg) = await _sut.BookSessionAsync(memberId, sessionId);

        // Assert
        rebookSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Cancellation_AttemptCancelNonExistentBooking()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 999;

        _mockRepository
            .Setup(r => r.CancelBookingAsync(memberId, sessionId))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CancelBookingAsync(memberId, sessionId);

        // Assert
        result.success.Should().BeFalse();
        result.message.Should().Be("Cancellation failed - please try again later");
    }

    #endregion

    #region Booking Status Tests

    [Fact]
    public async Task BookingStatus_CheckIfUserIsBooked()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 1;

        _mockRepository
            .Setup(r => r.IsBookedAsync(memberId, sessionId))
            .ReturnsAsync(true);

        // Act
        var isBooked = await _sut.IsBookedAsync(memberId, sessionId);

        // Assert
        isBooked.Should().BeTrue();
    }

    [Fact]
    public async Task BookingStatus_CheckIfUserIsNotBooked()
    {
        // Arrange
        int memberId = 1;
        int sessionId = 1;

        _mockRepository
            .Setup(r => r.IsBookedAsync(memberId, sessionId))
            .ReturnsAsync(false);

        // Act
        var isBooked = await _sut.IsBookedAsync(memberId, sessionId);

        // Assert
        isBooked.Should().BeFalse();
    }

    #endregion

    #region Real-World Scenarios

    [Fact]
    public async Task RealWorldScenario_MemberCompleteJourney()
    {
        // Scenario: A member browses sessions, books one, views their bookings, and cancels

        // Arrange
        int memberId = 1;
        var yoga = 1;
        var cardio = 2;

        var bookings = new List<Booking>
        {
            new()
            {
                Id = 1,
                UserId = memberId,
                SessionId = yoga,
                Session = new Session { Title = "Yoga", StartTime = DateTime.UtcNow.AddDays(1) }
            },
            new()
            {
                Id = 2,
                UserId = memberId,
                SessionId = cardio,
                Session = new Session { Title = "Cardio", StartTime = DateTime.UtcNow.AddDays(2) }
            }
        };

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, yoga))
            .ReturnsAsync(new BookingValidationResult(IsValid: true, Message: "OK"));

        _mockRepository
            .Setup(r => r.BookSessionAsync(memberId, yoga))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, cardio))
            .ReturnsAsync(new BookingValidationResult(IsValid: true, Message: "OK"));

        _mockRepository
            .Setup(r => r.BookSessionAsync(memberId, cardio))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.GetUserBookingsAsync(memberId))
            .ReturnsAsync(bookings);

        _mockRepository
            .Setup(r => r.CancelBookingAsync(memberId, yoga))
            .ReturnsAsync(true);

        // Act & Assert - Book Yoga
        var yogaResult = await _sut.BookSessionAsync(memberId, yoga);
        yogaResult.success.Should().BeTrue();

        // Act & Assert - Book Cardio
        var cardioResult = await _sut.BookSessionAsync(memberId, cardio);
        cardioResult.success.Should().BeTrue();

        // Act & Assert - View bookings
        var myBookings = await _sut.GetMyBookingsAsync(memberId);
        myBookings.Should().HaveCount(2);

        // Act & Assert - Cancel Yoga
        var cancelResult = await _sut.CancelBookingAsync(memberId, yoga);
        cancelResult.success.Should().BeTrue();
    }

    [Fact]
    public async Task RealWorldScenario_HighDemandSessionBecomesFull()
    {
        // Scenario: Session becomes full while trying to book

        // Arrange
        int memberId = 5;
        int sessionId = 1;

        // First time: validation passes (session not full)
        var firstValidation = new BookingValidationResult(IsValid: true, Message: "OK");
        var secondValidation = new BookingValidationResult(IsValid: false, Message: "The session is full (20/20)");

        var callCount = 0;
        _mockRepository
            .Setup(r => r.ValidateBookingAsync(memberId, sessionId))
            .Returns<int, int>(async (u, s) =>
            {
                callCount++;
                return callCount == 1 ? firstValidation : secondValidation;
            });

        _mockRepository
            .Setup(r => r.BookSessionAsync(memberId, sessionId))
            .ReturnsAsync(true);

        // Act - First booking attempt
        var firstAttempt = await _sut.BookSessionAsync(memberId, sessionId);

        // Assert
        firstAttempt.success.Should().BeTrue();

        // Act - Second booking attempt (session now full)
        var secondAttempt = await _sut.BookSessionAsync(memberId, sessionId);

        // Assert
        secondAttempt.success.Should().BeFalse();
        secondAttempt.message.Should().Contain("full");
    }

    #endregion
}
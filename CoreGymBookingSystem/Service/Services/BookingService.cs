using DAL.Entities;
using DAL.Repositories.Interfaces;
using Service.Interfaces;

namespace Service.Services;
public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Session>> GetMyBookingsAsync(int userId)
    {
        var bookings = await _repository.GetUserBookingsAsync(userId);
        return bookings
            .Select(b => b.Session!)
            .ToList();
    }

    public async Task<(bool success, string message)> BookSessionAsync(int userId, int sessionId)
    {
        var validation = await _repository.ValidateBookingAsync(userId, sessionId);
        if (!validation.IsValid)
            return (false, validation.Message);

        var success = await _repository.BookSessionAsync(userId, sessionId);

        if (success)
            return (true, "Booking successful!");
        else
            return (false, "Booking failed - please try again later");
    }

    public async Task<(bool success, string message)> CancelBookingAsync(int userId, int sessionId)
    {
        var success = await _repository.CancelBookingAsync(userId, sessionId);

        if (success)
            return (true, "Booking cancelled!");
        else
            return (false, "Cancellation failed - please try again later");
    }

    public async Task<bool> IsBookedAsync(int userId, int sessionId)
    {
        return await _repository.IsBookedAsync(userId, sessionId);
    }

    public async Task<(bool valid, string message)> ValidateAsync(int userId, int sessionId)
    {
        var result = await _repository.ValidateBookingAsync(userId, sessionId);
        return (result.IsValid, result.Message);
    }
}

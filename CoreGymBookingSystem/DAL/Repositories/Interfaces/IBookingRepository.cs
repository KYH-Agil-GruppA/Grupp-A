using DAL.Entities;
using DAL.Models;

namespace DAL.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetUserBookingsAsync(int userId);
    Task<Booking?> GetBookingAsync(int userId, int sessionId);
    Task<bool> IsBookedAsync(int userId, int sessionId);
    Task<bool> BookSessionAsync(int userId, int sessionId);
    Task<bool> CancelBookingAsync(int userId, int sessionId);
    Task<BookingValidationResult> ValidateBookingAsync(int userId, int sessionId);
}
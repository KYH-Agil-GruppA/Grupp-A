using DAL.Entities;

namespace Service.Interfaces;
public interface IBookingService
{
    Task<List<Session>> GetMyBookingsAsync(int userId);
    Task<(bool success, string message)> BookSessionAsync(int userId, int sessionId);
    Task<(bool success, string message)> CancelBookingAsync(int userId, int sessionId);
    Task<bool> IsBookedAsync(int userId, int sessionId);
    Task<(bool valid, string message)> ValidateAsync(int userId, int sessionId);
}

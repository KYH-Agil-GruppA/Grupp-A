using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

/// <summary>
/// Represents a booking of a session by a user.
/// </summary>
public class Booking
{
    /// <summary>
    /// Unique identifier for the booking.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The user who made the booking.
    /// </summary>
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    /// <summary>
    /// The session that was booked.
    /// </summary>
    public int SessionId { get; set; }

    [ForeignKey(nameof(SessionId))]
    public Session? Session { get; set; }

    /// <summary>
    /// When the booking was made.
    /// </summary>
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Status of the booking (e.g., "Confirmed", "Cancelled").
    /// </summary>
    public string Status { get; set; } = "Confirmed";
}
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

/// <summary>
/// A workout session.
/// </summary>
public class Session
{
    /// <summary>
    /// Unique identifier of the session.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Summary of the session.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Summary of the activities.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Summary of the activities.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Maximum allowed bookings.
    /// </summary>
    public int MaxParticipants { get; set; } = default;

    /// <summary>
    /// The instructor of the session.
    /// </summary>
    public User Instructor { get; set; } = null!;

    /// <summary>
    /// Users who have booked this session.
    /// </summary>
    public List<User> Bookings { get; set; } = [];

    /// <summary>
    /// The time the session starts.
    /// </summary>
    public DateTime StartTime { get; set; } = default;

    /// <summary>
    /// The time the session ends.
    /// </summary>
    public DateTime EndTime { get; set; } = default;

    /// <summary>
    /// The amount of people that have booked this session.
    /// </summary>
    [NotMapped, Obsolete( "Use Bookings.Count instead." )]
    public int CurrentBookings { get; set; }

    /// <summary>
    /// The day of the week the session starts.
    /// </summary>
    [NotMapped, Obsolete( "Use StartTime.DayOfWeek instead." )]
    public string DayOfWeek => StartTime.DayOfWeek.ToString();

    public bool IsDeleted { get; set; } 
}

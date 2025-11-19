using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

/// <summary>
/// A workout session.
/// </summary>
public class Session
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int MaxParticipants { get; set; } = default;

    /// <summary>
    /// Foreign key to the instructor user.
    /// </summary>
    public int? InstructorId { get; set; }

    /// <summary>
    /// The instructor of the session.
    /// </summary>
    [ForeignKey(nameof(InstructorId))]
    public User? Instructor { get; set; }

    /// <summary>
    /// Bookings for this session (many-to-one relationship).
    /// </summary>
    public List<Booking> Bookings { get; set; } = [];

    public DateTime StartTime { get; set; } = default;
    public DateTime EndTime { get; set; } = default;

    /// <summary>
    /// Soft delete flag for the session.
    /// </summary>
    public bool IsDeleted { get; set; }  
}

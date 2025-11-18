using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class Notification
{
    public int Id { get; set; }
    public int RecipientId { get; set; }

    [ForeignKey(nameof(RecipientId))]
    public User? Recipient { get; set; }

    public bool Read { get; set; } = false;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Type of notification (e.g., "Booking", "Cancellation", "Reminder").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// When the notification was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
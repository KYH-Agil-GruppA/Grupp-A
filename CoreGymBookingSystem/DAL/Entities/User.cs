using Microsoft.AspNetCore.Identity;

namespace DAL.Entities;

public class User : IdentityUser<int>
{
    /// <summary>
    /// All bookings made by this user.
    /// </summary>
    public List<Booking> Bookings { get; set; } = [];

    /// <summary>
    /// All sessions this user instructs (if they are a trainer).
    /// </summary>
    public List<Session> InstructedSessions { get; set; } = [];
}
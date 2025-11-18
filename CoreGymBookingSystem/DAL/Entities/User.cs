using Microsoft.AspNetCore.Identity;
using DAL.Entities;

namespace DAL.Entities;


/// <summary>
/// A gym account.
/// </summary>
public class User : IdentityUser<int>
{
    /// <summary>
    /// All bookings made by this user.
    /// </summary>
    public List<Session> Bookings { get; set; } = [];
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public int? MembershipTypeId { get; set; }
}

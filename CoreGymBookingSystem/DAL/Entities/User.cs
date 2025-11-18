using Microsoft.AspNetCore.Identity;

namespace DAL.Entities;


/// <summary>
/// A gym account.
/// </summary>
public class User : IdentityUser<int>
{
    /// <summary>
    /// All bookings made by this user.
    /// </summary>
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;    

  

    public List<Session> Bookings { get; set; } = [];

    public bool IsDeleted { get; set; }
}

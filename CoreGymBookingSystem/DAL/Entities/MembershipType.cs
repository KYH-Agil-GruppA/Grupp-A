using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

public class MembershipType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [Precision(18, 2)]
    public decimal Price { get; set; }

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

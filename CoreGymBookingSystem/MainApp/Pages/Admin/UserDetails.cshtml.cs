using DAL.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UserDetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public UserDetailsModel(
        ApplicationDbContext db,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
    }

    public UserDetailsVm User { get; private set; } = default!;

    public class UserDetailsVm
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string[] Roles { get; set; } = Array.Empty<string>();
        public bool IsLocked { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDetailsVm
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Address = u.Address,
                City = u.City,
                Country = u.Country,
                UserName = u.UserName ?? "(no username)",
                Email = u.Email ?? "",
                Roles = _db.UserRoles.AsNoTracking().Where(ur => ur.UserId == u.Id)
                    .Join(_db.Roles.AsNoTracking(),
                          ur => ur.RoleId,
                          r => r.Id,
                          (ur, r) => r.Name!)
                    .ToArray(),
                IsLocked = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow
            })
            .SingleOrDefaultAsync(ct);

        if (user == null)
            return NotFound();

        User = user;
        return Page();
    }
}

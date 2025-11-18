using DAL.DbContext;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public DashboardModel(ApplicationDbContext db,
                          RoleManager<IdentityRole<int>> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
    }

    // NOTIFICATIONS IF SUCCES
    [TempData]
    public string? StatusMessage { get; set; }

    public int TotalUsers { get; private set; }
    public int MembersCount { get; private set; }
    public int TrainersCount { get; private set; }
    public List<SimpleUserRow> LatestUsers { get; private set; } = new();

    public string? SearchTerm { get; private set; }

    public class SimpleUserRow
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

    public async Task OnGetAsync(string? searchTerm, CancellationToken ct)
    {
        SearchTerm = searchTerm;

        TotalUsers = await _db.Users.AsNoTracking().CountAsync(ct);
        MembersCount = await CountInRoleAsync("Member", ct);
        TrainersCount = await CountInRoleAsync("Trainer", ct);

        var memberId = (await _roleManager.FindByNameAsync("Member"))?.Id;
        var trainerId = (await _roleManager.FindByNameAsync("Trainer"))?.Id;
        var adminId = (await _roleManager.FindByNameAsync("Admin"))?.Id;

        var targetUserIds = _db.UserRoles.AsNoTracking()
            .Where(ur =>
                (memberId != null && ur.RoleId == memberId)
                || (trainerId != null && ur.RoleId == trainerId)
                || (adminId != null && ur.RoleId == adminId)
            )
            .Select(ur => ur.UserId);


        var query = _db.Users.AsNoTracking()
            .Where(u => targetUserIds.Contains(u.Id));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            int idValue;
            bool isIdSearch = int.TryParse(term, out idValue);

            query = query.Where(u =>
                (isIdSearch && u.Id == idValue)
                || (u.FirstName != null && u.FirstName.Contains(term))
                || (u.LastName != null && u.LastName.Contains(term))
                || _db.UserRoles.AsNoTracking()
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_db.Roles.AsNoTracking(),
                          ur => ur.RoleId,
                          r => r.Id,
                          (ur, r) => r.Name)
                    .Any(roleName => roleName.Contains(term))
            );
        }

        LatestUsers = await query
            .OrderByDescending(u => u.Id)
            .Take(20)
            .Select(u => new SimpleUserRow
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
                          .Join(_db.Roles.AsNoTracking(), ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
                          .ToArray(),
                IsLocked = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow
            })
            .ToListAsync(ct);
    }

    private async Task<int> CountInRoleAsync(string roleName, CancellationToken ct)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role is null) return 0;
        return await _db.UserRoles.AsNoTracking().CountAsync(ur => ur.RoleId == role.Id, ct);
    }
}

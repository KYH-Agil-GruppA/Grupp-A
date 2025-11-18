using DAL.DbContext;
using MainApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;

namespace WebApp.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UserDetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public readonly IUserService _userService;

    public UserDetailsModel(
        ApplicationDbContext db,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
    }

    //  message shown to admin after actions
    [TempData]
    public string? StatusMessage { get; set; }

    public UserDetailsVm User { get; private set; } = default!;

    [BindProperty]
    public EditUserInput Input { get; set; } = new();

   [BindProperty]
    public UserViewModel UserViewModel { get; set; }

    // Roles UI
    public List<string> AvailableRoles { get; private set; } = new();

    [BindProperty]
    public string? SelectedRole { get; set; }

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

    public class EditUserInput
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
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

        var userDelete = await _db.Users
        .AsNoTracking()
        .Where(u => u.Id == id)
        .Select(u => new UserViewModel
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Address = u.Address,
            City = u.City,
            Country = u.Country,

        })
        .SingleOrDefaultAsync(ct);

        if (user == null)
            return NotFound();

        if (userDelete != null)
            return NotFound();

        User = user;

        // Pre-fill edit form
        Input = new EditUserInput
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Address = user.Address,
            City = user.City,
            Country = user.Country
        };

        // Load roles for dropdown
        AvailableRoles = await _roleManager.Roles
            .Select(r => r.Name!)
            .OrderBy(n => n)
            .ToListAsync(ct);

        SelectedRole = User.Roles.FirstOrDefault();

        return Page();
    }

    // UPDATE user (basic info)
    public async Task<IActionResult> OnPostSaveAsync(CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == Input.Id, ct);

        if (user == null)
            return NotFound();

        user.FirstName = Input.FirstName ?? "";
        user.LastName = Input.LastName ?? "";
        user.Email = Input.Email;
        user.Address = Input.Address ?? "";
        user.City = Input.City ?? "";
        user.Country = Input.Country ?? "";

        await _db.SaveChangesAsync(ct);

        StatusMessage = "Member details have been updated successfully.";

        return RedirectToPage(new { id = Input.Id });
    }

    // LOCK account
    public async Task<IActionResult> OnPostLockAsync(int id, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null)
            return NotFound();

        user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        await _db.SaveChangesAsync(ct);

        StatusMessage = "The member account has been locked.";
        return RedirectToPage(new { id });
    }

    // UNLOCK account
    public async Task<IActionResult> OnPostUnlockAsync(int id, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null)
            return NotFound();

        user.LockoutEnd = null;
        await _db.SaveChangesAsync(ct);

        StatusMessage = "The member account has been unlocked.";
        return RedirectToPage(new { id });
    }

    // DELETE user
    public IActionResult OnPostDeleteAsync(int id)
    {
        var users = _userService.GetUser(UserViewModel.Id);
        if (users != null)
        {
            users.IsDeleted = true;
            _userService.Update(users);

        }

        StatusMessage = "The member account has been deleted.";
        return RedirectToPage("/Admin/Dashboard");





    }

    // CHANGE ROLE
    public async Task<IActionResult> OnPostChangeRoleAsync(int id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(SelectedRole))
        {
            StatusMessage = "Please select a role.";
            return RedirectToPage(new { id });
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null)
            return NotFound();

        var role = await _roleManager.FindByNameAsync(SelectedRole);
        if (role == null)
        {
            StatusMessage = $"Role '{SelectedRole}' does not exist.";
            return RedirectToPage(new { id });
        }

        // Remove all current roles
        var currentRoles = await _db.UserRoles
            .Where(ur => ur.UserId == id)
            .ToListAsync(ct);

        _db.UserRoles.RemoveRange(currentRoles);

        // Assign the new role
        _db.UserRoles.Add(new IdentityUserRole<int>
        {
            UserId = id,
            RoleId = role.Id
        });

        await _db.SaveChangesAsync(ct);

        StatusMessage = $"Role has been changed to '{SelectedRole}'.";

        return RedirectToPage(new { id });
    }
}

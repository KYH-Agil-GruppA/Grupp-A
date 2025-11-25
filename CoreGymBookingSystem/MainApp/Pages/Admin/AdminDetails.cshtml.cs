using DAL.DbContext;
using MainApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AdminDetailsModel(
            ApplicationDbContext db,
            IUserService userService,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _db = db;
            _userService = userService;
            _roleManager = roleManager;
        }

        [TempData]
        public string? StatusMessage { get; set; }

        public AdminDetailsVm Admin { get; private set; } = default!;

        [BindProperty]
        public EditAdminInput Input { get; set; } = new();

        [BindProperty]
        public UserViewModel UserViewModel { get; set; } = new();

        public List<string> AvailableRoles { get; private set; } = new();

        [BindProperty]
        public string? SelectedRole { get; set; }

        public class AdminDetailsVm
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

        public class EditAdminInput
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "First name is required")]
            [StringLength(100, ErrorMessage = "First name can't be longer than 100 characters")]
            public string? FirstName { get; set; }

            [Required(ErrorMessage = "Last name is required")]
            [StringLength(100, ErrorMessage = "Last name can't be longer than 100 characters")]
            public string? LastName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Address is required")]
            [StringLength(200, ErrorMessage = "Address can't be longer than 200 characters")]
            public string? Address { get; set; }

            [Required(ErrorMessage = "City is required")]
            [StringLength(100, ErrorMessage = "City can't be longer than 100 characters")]
            public string? City { get; set; }

            [Required(ErrorMessage = "Country is required")]
            [StringLength(100, ErrorMessage = "Country can't be longer than 100 characters")]
            public string? Country { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
        {
            await LoadAdminAsync(id, ct);
            if (Admin == null)
                return NotFound();

            return Page();
        }

        private async Task LoadAdminAsync(int id, CancellationToken ct)
        {
            var admin = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new AdminDetailsVm
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Address = u.Address,
                    City = u.City,
                    Country = u.Country,
                    UserName = u.UserName ?? "(no username)",
                    Email = u.Email ?? "",
                    Roles = _db.UserRoles.AsNoTracking()
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_db.Roles.AsNoTracking(),
                              ur => ur.RoleId,
                              r => r.Id,
                              (ur, r) => r.Name!)
                        .ToArray(),
                    IsLocked = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow
                })
                .SingleOrDefaultAsync(ct);

            if (admin == null)
            {
                Admin = null!;
                AvailableRoles = new List<string>();
                return;
            }

            Admin = admin;

            UserViewModel = new UserViewModel
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Address = admin.Address,
                City = admin.City,
                Country = admin.Country
            };

            // Pre-fill edit form
            Input = new EditAdminInput
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email,
                Address = admin.Address,
                City = admin.City,
                Country = admin.Country
            };

            // Roller för dropdown
            AvailableRoles = await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToListAsync(ct);

            SelectedRole = Admin.Roles.FirstOrDefault();
        }

        // UPDATE admin (basic info)
        public async Task<IActionResult> OnPostSaveAsync(CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await LoadAdminAsync(Input.Id, ct);
                return Page();
            }

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

            StatusMessage = "Admin details have been updated successfully.";

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

            StatusMessage = "The admin account has been locked.";
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

            StatusMessage = "The admin account has been unlocked.";
            return RedirectToPage(new { id });
        }

        // DELETE admin 
        public IActionResult OnPostDelete(int id)
        {
            var user = _userService.GetUser(UserViewModel.Id);
            if (user != null)
            {
                user.IsDeleted = true;
                _userService.Update(user);
            }

            StatusMessage = "The admin account has been deleted.";
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

            var currentRoles = await _db.UserRoles
                .Where(ur => ur.UserId == id)
                .ToListAsync(ct);

            _db.UserRoles.RemoveRange(currentRoles);

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
}

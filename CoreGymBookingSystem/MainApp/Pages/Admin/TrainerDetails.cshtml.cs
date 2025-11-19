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
    public class TrainerDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public TrainerDetailsModel(
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

        public TrainerDetailsVm Trainer { get; private set; } = default!;

        public List<TrainerSessionVm> Sessions { get; private set; } = new();

        [BindProperty]
        public UserViewModel UserViewModel { get; set; } = new();

        [BindProperty]
        public EditTrainerInput Input { get; set; } = new();

        // Roller för dropdown
        public List<string> AvailableRoles { get; private set; } = new();

        [BindProperty]
        public string? SelectedRole { get; set; }

        public class TrainerDetailsVm
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

        public class TrainerSessionVm
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Category { get; set; } = "";
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public int MaxParticipants { get; set; }
        }

        public class EditTrainerInput
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
            await LoadTrainerAsync(id, ct);
            if (Trainer == null)
                return NotFound();

            return Page();
        }

        private async Task LoadTrainerAsync(int id, CancellationToken ct)
        {
            // Hämta tränaren
            var trainer = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new TrainerDetailsVm
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

            if (trainer == null)
            {
                Trainer = null!;
                Sessions = new List<TrainerSessionVm>();
                AvailableRoles = new List<string>();
                return;
            }

            Trainer = trainer;

            // Fyll UserViewModel för delete
            UserViewModel = new UserViewModel
            {
                Id = trainer.Id,
                FirstName = trainer.FirstName,
                LastName = trainer.LastName,
                Address = trainer.Address,
                City = trainer.City,
                Country = trainer.Country
            };

            // Pre-fill edit form
            Input = new EditTrainerInput
            {
                Id = trainer.Id,
                FirstName = trainer.FirstName,
                LastName = trainer.LastName,
                Email = trainer.Email,
                Address = trainer.Address,
                City = trainer.City,
                Country = trainer.Country
            };

            // Hämta tränarens pass (sessions)
            Sessions = await _db.Sessions
                .AsNoTracking()
                .Where(s => s.InstructorId == id)
                .OrderBy(s => s.StartTime)
                .Select(s => new TrainerSessionVm
                {
                    Id = s.Id,
                    Title = s.Title,
                    Category = s.Category,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    MaxParticipants = s.MaxParticipants
                })
                .ToListAsync(ct);

            // Hämta roller för dropdown
            AvailableRoles = await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToListAsync(ct);

            SelectedRole = Trainer.Roles.FirstOrDefault();
        }

        // UPDATE trainer (basic info)
        public async Task<IActionResult> OnPostSaveAsync(CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                // Ladda om Trainer + Sessions + roller så sidan kan visas
                await LoadTrainerAsync(Input.Id, ct);
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

            StatusMessage = "Trainer details have been updated successfully.";

            return RedirectToPage(new { id = Input.Id });
        }

        // LOCK tränare
        public async Task<IActionResult> OnPostLockAsync(int id, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user == null)
                return NotFound();

            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _db.SaveChangesAsync(ct);

            StatusMessage = "The trainer account has been locked.";
            return RedirectToPage(new { id });
        }

        // UNLOCK tränare
        public async Task<IActionResult> OnPostUnlockAsync(int id, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user == null)
                return NotFound();

            user.LockoutEnd = null;
            await _db.SaveChangesAsync(ct);

            StatusMessage = "The trainer account has been unlocked.";
            return RedirectToPage(new { id });
        }

        // DELETE tränare (soft delete via IUserService)
        public IActionResult OnPostDelete(int id)
        {
            var user = _userService.GetUser(UserViewModel.Id);
            if (user != null)
            {
                user.IsDeleted = true;
                _userService.Update(user);
            }

            StatusMessage = "The trainer account has been deleted.";
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

            // Ta bort alla nuvarande roller
            var currentRoles = await _db.UserRoles
                .Where(ur => ur.UserId == id)
                .ToListAsync(ct);

            _db.UserRoles.RemoveRange(currentRoles);

            // Lägg till den nya rollen
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

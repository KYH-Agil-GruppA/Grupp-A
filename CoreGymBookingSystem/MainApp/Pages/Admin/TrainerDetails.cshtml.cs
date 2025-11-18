using DAL.DbContext;
using MainApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using Service.Interfaces;

namespace WebApp.Pages.Admin;

[Authorize(Roles = "Admin")]
public class TrainerDetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public readonly IUserService _userService;

    [BindProperty]
    public UserViewModel UserViewModel { get; set; }
    public TrainerDetailsModel(ApplicationDbContext db, IUserService userService)
    {
        _db = db;
        _userService = userService;

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
                    .ToArray()
            })
            .SingleOrDefaultAsync(ct);

        if (user == null)
            return NotFound();

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

        if(userDelete == null) return NotFound();   
        User = user;
        return Page();
    }

  
    public IActionResult OnPostDelete(int id)
    {
        var user = _userService.GetUser(UserViewModel.Id);
        if (user != null)
        {
            user.IsDeleted = true;
            _userService.Update(user);
        }
        return RedirectToPage("/Admin/Dashboard");
    }

    
}

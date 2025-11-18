using DAL.Entities;
using DAL.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Pages.Member
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public MembershipType? Member { get; set; }
        public User? CurrentUser { get; set; }

        public ProfileModel(
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task OnGet()
        {
        
            CurrentUser = await _userManager.GetUserAsync(User);

            if (CurrentUser == null)
                return;

            Member = await _context.MembershipTypes
                .FirstOrDefaultAsync(m => m.Id == CurrentUser.MembershipTypeId);
        }
    }
}

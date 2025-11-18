using DAL.DbContext;
using DAL.Entities;
using MainApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages.Membership
{
    public class SignupModel : PageModel
    {
        private readonly IMembershipService _service;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public MembershipSignupViewModel Form { get; set; }

        public SignupModel(IMembershipService service,
                           UserManager<User> userManager,
                           ApplicationDbContext context)
        {
            _service = service;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var membership = await _service.GetMembership(id);

            if (membership == null)
                return NotFound();

            Form = new MembershipSignupViewModel
            {
                MembershipId = membership.Id,
                MembershipName = membership.Name,
                Price = membership.Price
            };

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);

            // Save membership type to user
            user.MembershipTypeId = Form.MembershipId;
            user.FirstName = Form.FirstName;
            user.LastName = Form.LastName;
            user.Address = Form.Address;
            user.PostalCode = Form.PostalCode;
            user.City = Form.City;
            user.PhoneNumber = Form.PhoneNumber;
            user.Email = Form.Email;

            await _userManager.UpdateAsync(user);

            // TODO: Payment Logic Placeholder 

            return RedirectToPage("/Member/Profile");
        }
    }

}

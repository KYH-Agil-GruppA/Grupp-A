using DAL.Entities;
using MainApp.ViewModel.Membership;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages.Membership
{
    public class SignupModel : PageModel
    {
        private readonly IMembershipService _membershipService;

        public SignupModel(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [BindProperty]
        public MembershipSignupViewModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var membership = await _membershipService.GetByIdAsync(id);

            if (membership == null)
                return RedirectToPage("/Error");

            Input = new MembershipSignupViewModel
            {
                MembershipTypeId = membership.Id,
                MembershipName = membership.Name,
                ImageUrl = membership.ImageUrl,
                Price = membership.Price,
                Description = membership.Description
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
                return Page();

            if (Input.MembershipTypeId == 0)
                Input.MembershipTypeId = id;


            return RedirectToPage("/Membership/Summary", new
            {
                id = Input.MembershipTypeId,
                first = Input.FirstName,
                last = Input.LastName,
                email = Input.Email,
                address = Input.Address,
                phone = Input.Phone
            });
        }
    }
}

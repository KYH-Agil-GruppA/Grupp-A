using MainApp.ViewModel.Membership;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages.Membership
{
    public class SummaryModel : PageModel
    {
        private readonly IMembershipService _membershipService;

        public SummaryModel(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public MembershipSummaryVM Info { get; set; }

        public async Task<IActionResult> OnGetAsync(
            int id, string first, string last,
            string email, string address, string phone)

        {

            var membership = await _membershipService.GetByIdAsync(id);

            if (membership == null)
            {
                return RedirectToPage("/Error");
            }

            Info = new MembershipSummaryVM
            {
                MembershipName = membership.Name,
                Price = membership.Price,
                Description = membership.Description,
                ImageUrl = membership.ImageUrl,

                FirstName = first,
                LastName = last,
                Email = email,
                Address = address,
                Phone = phone
            };

            return Page();
        }
    }
}

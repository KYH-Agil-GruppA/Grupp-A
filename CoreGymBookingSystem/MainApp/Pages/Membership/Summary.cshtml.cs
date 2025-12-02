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
         string email, string address, string phone,
         DateTime startDate)
        {
            var membership = await _membershipService.GetByIdAsync(id);

            if (membership == null)
                return RedirectToPage("/Error");

            Info = new MembershipSummaryVM
            {
                MembershipTypeId = membership.Id,
                MembershipName = membership.Name,
                Description = membership.Description,
                ImageUrl = membership.ImageUrl,
                Price = membership.Price,

                FirstName = first,
                LastName = last,
                Email = email,
                Address = address,
                Phone = phone,

                StartDate = startDate
            };

            return Page();
        }

    }
}

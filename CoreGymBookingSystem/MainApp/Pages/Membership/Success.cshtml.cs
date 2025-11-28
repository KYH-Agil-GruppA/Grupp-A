using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages.Membership
{
    public class SuccessModel : PageModel
    {
        private readonly IMembershipService _membershipService;

        public SuccessModel(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public SuccessInfoModel Info { get; set; }

        public class SuccessInfoModel
        {
            public string MembershipName { get; set; }
            public decimal Price { get; set; }
            public DateTime StartDate { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(
            int id,
            DateTime date,
            string first,
            string last,
            string email,
            string phone)
        {
            var membership = await _membershipService.GetByIdAsync(id);
            if (membership == null)
                return RedirectToPage("/Error");

            Info = new SuccessInfoModel
            {
                MembershipName = membership.Name,
                Price = membership.Price,
                StartDate = date,
                FirstName = first,
                LastName = last,
                Email = email,
                Phone = phone
            };

            return Page();
        }
    }
}

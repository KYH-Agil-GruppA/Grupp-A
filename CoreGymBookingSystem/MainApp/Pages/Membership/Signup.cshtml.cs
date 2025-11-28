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
        public SignupInputModel Input { get; set; }

        public class SignupInputModel
        {
            public int MembershipTypeId { get; set; }
            public string MembershipName { get; set; } = "";
            public string Description { get; set; } = "";
            public string ImageUrl { get; set; } = "";
            public decimal Price { get; set; }

            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Email { get; set; } = "";
            public string Address { get; set; } = "";
            public string Phone { get; set; } = "";
            public DateTime StartDate { get; set; }
        }

        public async Task<IActionResult> OnGet(int id, DateTime startDate)
        {
            var membership = await _membershipService.GetByIdAsync(id);

            if (membership == null)
                return RedirectToPage("/Error");

            Input = new SignupInputModel
            {
                MembershipTypeId = id,
                MembershipName = membership.Name,
                Description = membership.Description,
                Price = membership.Price,
                ImageUrl = membership.ImageUrl,
                StartDate = startDate
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            return RedirectToPage("/Membership/Success", new
            {
                id = Input.MembershipTypeId,
                date = Input.StartDate.ToString("yyyy-MM-dd"),
                first = Input.FirstName,
                last = Input.LastName,
                email = Input.Email,
                phone = Input.Phone
            });
        }
    }
}

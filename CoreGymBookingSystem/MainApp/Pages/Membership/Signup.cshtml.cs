using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages.Membership
{
    public class SignupModel : PageModel
    {
        private readonly IMembershipService _membershipService;
        private readonly IMembershipPurchaseService _purchaseService;

        public SignupModel(IMembershipService membershipService,
                           IMembershipPurchaseService purchaseService)
        {
            _membershipService = membershipService;
            _purchaseService = purchaseService;
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

        // GET
        public async Task<IActionResult> OnGet(int id, DateOnly startDate)
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
                StartDate = startDate.ToDateTime(TimeOnly.MinValue)
            };

            return Page();
        }

        // POST
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // 1. Past date validation
            if (Input.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "Start date cannot be in the past.");
                return Page();
            }

          

            // 2. Duplicate check
            var start = DateOnly.FromDateTime(Input.StartDate);
            var existing = await _purchaseService.ExistsAsync(Input.Email, start);
            if (existing)
            {
                ModelState.AddModelError(string.Empty, "You have already booked this date.");
                return Page();
            }

            // 3. Save
            var purchase = new MembershipPurchase
            {
                MembershipTypeId = Input.MembershipTypeId,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                Address = Input.Address,
                Phone = Input.Phone,
                StartDate = start,
                PurchaseDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _purchaseService.AddAsync(purchase);

            // 4. Redirect
            return RedirectToPage("/Membership/Success", new
            {
                id = Input.MembershipTypeId,
                date = start,
                first = Input.FirstName,
                last = Input.LastName,
                email = Input.Email,
                phone = Input.Phone
            });
        }
    }
}
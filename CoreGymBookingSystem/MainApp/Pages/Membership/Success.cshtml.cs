using DAL.Entities;
using MainApp.ViewModel.Membership;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages.Membership
{
    public class SuccessModel : PageModel
    {
        private readonly IMembershipService _service;

        public SuccessModel(IMembershipService service)
        {
            _service = service;
        }

        public MembershipSummaryVM Info { get; set; }

        public async Task<IActionResult> OnGetAsync(
            int id, string first, string last,
            string email, string address, string phone,
            DateTime startDate)
        {
            var membership = await _service.GetByIdAsync(id);

            if (membership == null)
                return RedirectToPage("/Error");

            Info = new MembershipSummaryVM
            {
                MembershipTypeId = id,
                MembershipName = membership.Name,
                Price = membership.Price,
                Description = membership.Description,
                ImageUrl = membership.ImageUrl,

                FirstName = first,
                LastName = last,
                Email = email,
                Address = address,
                Phone = phone,
                StartDate = startDate
            };

            var purchase = new MembershipPurchase
            {
                MembershipTypeId = id,
                FirstName = first,
                LastName = last,
                Email = email,
                Address = address,
                Phone = phone,
                StartDate = startDate
            };

            await _service.SavePurchaseAsync(purchase);

            return Page();
        }
    }
}

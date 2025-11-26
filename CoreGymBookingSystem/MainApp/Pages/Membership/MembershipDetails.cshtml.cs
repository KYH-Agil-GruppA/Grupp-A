using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages.Membership
{
    public class MembershipDetailsModel : PageModel
    {
        private readonly IMembershipService _service;

        public MembershipDetailsModel(IMembershipService service)
        {
            _service = service;
        }

        public MembershipType? Membership { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            Membership = await _service.GetByIdAsync(id);

            if (Membership == null)
            {
                return RedirectToPage("MembershipList");
            }

            return Page();
        }
    }
}

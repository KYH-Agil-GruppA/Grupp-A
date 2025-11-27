using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages.Membership
{
    public class IndexModel : PageModel
    {
        private readonly IMembershipService _membershipService;

        public IndexModel(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public IList<MembershipType> Memberships { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Memberships = await _membershipService.GetAllAsync();
            return Page();
        }
    }
}

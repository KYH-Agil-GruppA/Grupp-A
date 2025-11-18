using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages.Membership;

public class MembershipListModel : PageModel
{
    private readonly IMembershipService _service;

    public MembershipListModel(IMembershipService service)
    {
        _service = service;
    }

    public List<MembershipType> Memberships { get; set; } = new();

    public async Task OnGet()
    {
        Memberships = await _service.GetAllAsync();
    }
}

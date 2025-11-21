using DAL.DbContext;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;

namespace MainApp.Pages.Sessions;

public class SessionDetailsModel(
    ApplicationDbContext context,
    IBookingService bookingService,
    UserManager<User> userManager) : PageModel
{
    private readonly ApplicationDbContext _context = context;
    private readonly IBookingService _bookingService = bookingService;
    private readonly UserManager<User> _userManager = userManager;

    public Session Session { get; set; } = default!;
    public bool IsMember { get; set; } = false;
    public bool IsBooked { get; set; } = false;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var session = await _context.Sessions
            .Include(s => s.Instructor)
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) return NotFound();

        Session = session;

        var user = await _userManager.GetUserAsync(User);
        if (user != null && await _userManager.IsInRoleAsync(user, "Member"))
        {
            IsMember = true;
            IsBooked = await _bookingService.IsBookedAsync(user.Id, id);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostBookAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        var (success, message) = await _bookingService.BookSessionAsync(user.Id, id);
        StatusMessage = message;

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        var (success, message) = await _bookingService.CancelBookingAsync(user.Id, id);
        StatusMessage = message;

        return RedirectToPage(new { id });
    }
}
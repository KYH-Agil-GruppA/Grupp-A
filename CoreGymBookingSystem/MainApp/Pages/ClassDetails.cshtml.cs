using DAL.DTOs;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;

namespace MainApp.Pages;

public class ClassDetailsModel : PageModel
{
    private readonly ISessionService _sessionService;
    private readonly UserManager<User> _userManager;

    public ClassDetailsModel(
        ISessionService sessionService,
        UserManager<User> userManager)
    {
        _sessionService = sessionService;
        _userManager = userManager;
    }

    public List<SessionsDto> Sessions { get; set; } = new();
    public string Filter { get; set; } = "all";

    public async Task<IActionResult> OnGetAsync(string? filter)
    {
        Filter = string.IsNullOrWhiteSpace(filter) ? "all" : filter!.ToLowerInvariant();

        if (Filter == "all")
        {
            // Hämta alla sessioner och mappa till DTO (full kontroll över projektionen)
            var allSessions = await _sessionService.GetAllSessionsAsync();

            Sessions = allSessions.Select(s => new SessionsDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                Category = s.Category,
                DayOfWeek = s.StartTime.DayOfWeek.ToString(),
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                InstructorUserName = s.Instructor?.UserName,
                MaxParticipants = s.MaxParticipants,
                CurrentBookings = s.Bookings.Count
            }).ToList();
        }
        else
        {
            // Hämta redan färdiga DTO:er per kategori (bättre prestanda om servicen gör projektion i DB)
            Sessions = await _sessionService.GetSessionsByCategoryAsync(Filter);
        }

        return Page();
    }
}
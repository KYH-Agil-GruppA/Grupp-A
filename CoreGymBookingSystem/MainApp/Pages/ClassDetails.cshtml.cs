using DAL.DTOs;                 // <-- lägg till
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using System.Linq;

namespace MainApp.Pages
{
    public class ClassDetailsModel : PageModel
    {
        private readonly ISessionService _sessionService;

        public ClassDetailsModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public Session? Session { get; set; }

        public List<SessionsDto> Sessions { get; set; } = new();

        // För att markera aktiv knapp i vyn
        public string Filter { get; set; } = "all";

        // /ClassDetails?id=5&filter=fitness
        public async Task<IActionResult> OnGetAsync(int? id, string? filter)
        {
            // Sätt valt filter (default "all")
            Filter = string.IsNullOrWhiteSpace(filter) ? "all" : filter.ToLowerInvariant();

            // id är nullable nu, så detta funkar
            if (id.HasValue)
            {
                Session = await _sessionService.GetSessionByIdAsync(id.Value);
            }

            if (Filter == "all")
            {
                // Hämta alla (entities) och mappa till DTO här
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
                // Hämta redan projicerad lista från service (DTO-version)
                Sessions = await _sessionService.GetSessionsByCategoryAsync(Filter);
            }

            return Page();
        }
    }
}

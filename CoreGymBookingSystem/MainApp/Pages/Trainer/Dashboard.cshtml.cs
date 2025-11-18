using DAL.DTOs;
using MainApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
// Keep ONLY the correct interface namespace for your project:
using Services.Interfaces; // <-- If your ISessionService lives here, keep this line
// using Service.Interfaces; // <-- Remove this if it's the wrong/duplicate namespace
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MainApp.Pages.Trainer
{
    [Authorize(Roles = "Trainer")]
    public class DashboardModel : PageModel
    {
        private readonly ISessionService _sessionService;

        [BindProperty]
        public TrainerDashboardVm Vm { get; set; }
        public string Filter { get; set; } = "all";
        public string SuccessMessage { get; set; }
        public DateTime PrevWeekStart { get; set; }
        public DateTime NextWeekStart { get; set; }

        public DashboardModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> OnGet(DateTime? start, string? filter)
        {
            Vm = new TrainerDashboardVm();

            DateTime weekStart = GetWeekStart(start ?? DateTime.Today);
            Vm.WeekStart = weekStart;

            PrevWeekStart = weekStart.AddDays(-7);
            NextWeekStart = weekStart.AddDays(7);

            Filter = string.IsNullOrWhiteSpace(filter) ? "all" : filter.ToLowerInvariant();

            int instructorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Vm.SessionsDetailed = await _sessionService.GetDetailedForInstructorWeekAsync(instructorId, weekStart);

            if (TempData.ContainsKey("Success"))
            {
                SuccessMessage = TempData["Success"] as string;
            }

            return Page();
        }

        private DateTime GetWeekStart(DateTime anyDay)
        {
            int diff = (7 + (int)anyDay.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            return anyDay.Date.AddDays(-diff);
        }
    }
}

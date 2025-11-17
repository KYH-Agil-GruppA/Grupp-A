using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.DTOs;
using MainApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace MainApp.Pages.Trainer
{
    [Authorize(Roles = "Trainer")]
    public class AddClassModel : PageModel
    {
        private readonly ISessionService _sessionService;

        [BindProperty]
        public TrainerDashboardVm Vm { get; set; }

        public AddClassModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public void OnGet()
        {
            Vm = new TrainerDashboardVm();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Same day check
            if (Vm.Create.StartTime.Date != Vm.Create.EndTime.Date)
            {
                ModelState.AddModelError("", "End date must match start date.");
                return Page();
            }

            // Duration check
            var duration = Vm.Create.EndTime - Vm.Create.StartTime;
            if (duration.TotalHours < 1 || duration.TotalHours > 4)
            {
                ModelState.AddModelError("", "Class must be between 1 and 4 hours long.");
                return Page();
            }

            int instructorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var dto = new SessionCreateDto
            {
                Title = Vm.Create.Title,
                Description = Vm.Create.Description,
                StartTime = Vm.Create.StartTime,
                EndTime = Vm.Create.EndTime,
                MaxParticipants = Vm.Create.MaxParticipants,
                Category = Vm.Create.Category,
                InstructorId = instructorId
            };

            try
            {
                await _sessionService.CreateAsync(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }

            TempData["Success"] = "Class added successfully!";
            return RedirectToPage("/Trainer/Dashboard");
        }
    }
}

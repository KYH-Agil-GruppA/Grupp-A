using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.DTOs;
using DAL.Enums;

namespace MainApp.ViewModel
{
    public class TrainerDashboardVm
    {
        // Used by the dashboard timetable
        public IList<SessionsDto> SessionsDetailed { get; set; }

        // If you still use the simple list elsewhere, keep it; otherwise you can remove it
        public DateTime WeekStart { get; set; }

        // Optional: keep if you still use Create on other pages
        public CreateSessionVm Create { get; set; }

        public TrainerDashboardVm()
        {
            SessionsDetailed = new List<SessionsDto>();
            Create = new CreateSessionVm();
        }
    }

    public class CreateSessionVm
    {
        [Required] public string Title { get; set; }
        public string Description { get; set; }
        [Required] public DateTime StartTime { get; set; }
        [Required] public DateTime EndTime { get; set; }
        [Range(1, 200)] public int MaxParticipants { get; set; }
        [Required] public Category Category { get; set; }
    }
}

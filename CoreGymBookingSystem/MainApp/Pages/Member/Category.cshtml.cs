using MainApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace MainApp.Pages.Member
{
    [Authorize(Roles = "Member")]
    public class CategoryModel : PageModel
    {
        private readonly ISessionService _sessionService;

        [BindProperty(SupportsGet = true)]
        public string? SelectedCategory { get; set; }

        public List<SessionViewModel> SearchbyCategory { get; set; } = new();

        public CategoryModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task OnGetAsync()
        {
            await LoadSessions();
        }

        public async Task OnPostAsync()
        {
            await LoadSessions();
        }

        private async Task LoadSessions()
        {
            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                SearchbyCategory = new List<SessionViewModel>();
                return;
            }

            var sessions = await _sessionService.GetSessionsByCategoryAsync(SelectedCategory);
            SearchbyCategory = sessions.Select(s => new SessionViewModel
            {
                Title = s.Title,
                Description = s.Description,
                Category = s.Category
            }).ToList();
        }
    }
}
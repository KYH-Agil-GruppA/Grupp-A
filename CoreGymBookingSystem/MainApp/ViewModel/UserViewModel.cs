using DAL.Entities;

namespace MainApp.ViewModel
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;



        public List<SessionViewModel> Bookings { get; set; } = [];

        public bool IsDeleted { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MainApp.ViewModel
{
    public class MembershipSignupViewModel
    {
        public int MembershipId { get; set; }
        public string? MembershipName { get; set; }
        public decimal Price { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

}

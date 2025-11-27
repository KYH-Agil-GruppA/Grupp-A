namespace MainApp.ViewModel.Membership
{
    public class MembershipSummaryVM
    {
        public string MembershipName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int MembershipTypeId { get; set; }
    }

}

using DAL.Entities;
using MainApp.Pages.Membership;
using Moq;
using Service.Interfaces;
using Xunit;

namespace MembershipPlans.Test1.Pages
{
    public class MembershipListModelTests
    {
        private readonly Mock<IMembershipService> _serviceMock;
        private readonly MembershipListModel _page;

        public MembershipListModelTests()
        {
            _serviceMock = new Mock<IMembershipService>();
            _page = new MembershipListModel(_serviceMock.Object);
        }

        [Fact]
        public async Task OnGet_LoadsMemberships()
        {
            _serviceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<MembershipType>
                {
                    new MembershipType { Id = 1, Name = "Adult" },
                    new MembershipType { Id = 2, Name = "Student" }
                });

            await _page.OnGet();

            Assert.Equal(2, _page.Memberships.Count);
        }
    }
}

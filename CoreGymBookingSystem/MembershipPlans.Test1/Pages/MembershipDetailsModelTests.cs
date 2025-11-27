using DAL.Entities;
using MainApp.Pages.Membership;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Service.Interfaces;

namespace MembershipPlans.Test1.Pages
{
    public class MembershipDetailsModelTests
    {
        private readonly Mock<IMembershipService> _serviceMock;
        private readonly MembershipDetailsModel _page;

        public MembershipDetailsModelTests()
        {
            _serviceMock = new Mock<IMembershipService>();
            _page = new MembershipDetailsModel(_serviceMock.Object);
        }

        [Fact]
        public async Task OnGet_ReturnsPage_WhenMembershipExists()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(new MembershipType
                {
                    Id = 1,
                    Name = "Adult Membership"
                });

            var result = await _page.OnGet(1);

            Assert.IsType<PageResult>(result);
            Assert.NotNull(_page.Membership);
            Assert.Equal("Adult Membership", _page.Membership.Name);
        }

        [Fact]
        public async Task OnGet_Redirects_WhenNotFound()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync((MembershipType?)null);

            var result = await _page.OnGet(99);

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("MembershipList", redirect.PageName);
        }
    }
}

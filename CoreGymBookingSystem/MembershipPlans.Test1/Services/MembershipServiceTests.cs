using DAL.DbContext;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Services;
using Xunit;

namespace MembershipPlans.Test1.Services
{
    public class MembershipServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // new DB for each test
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllMemberships()
        {
            // Arrange
            using var context = CreateContext();
            context.MembershipTypes.AddRange(
                new MembershipType { Id = 1, Name = "Adult", Price = 399 },
                new MembershipType { Id = 2, Name = "Student", Price = 299 }
            );
            await context.SaveChangesAsync();

            var service = new MembershipService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Name == "Adult");
            Assert.Contains(result, x => x.Price == 299);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMembership_WhenExists()
        {
            // Arrange
            using var context = CreateContext();
            context.MembershipTypes.Add(new MembershipType
            {
                Id = 10,
                Name = "Senior",
                Price = 249
            });
            await context.SaveChangesAsync();

            var service = new MembershipService(context);

            // Act
            var result = await service.GetByIdAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Senior", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            using var context = CreateContext();
            var service = new MembershipService(context);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAL.DbContext;
using DAL.Entities;
using DAL.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApp.Pages.Admin;
using Service.Interfaces;
using Xunit;


namespace Tests
{
    public class TestHelpers
    {
        public static ApplicationDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        public static RoleManager<IdentityRole<int>> CreateRoleManager(IEnumerable<IdentityRole<int>> roles)
        {
            var store = new Mock<IRoleStore<IdentityRole<int>>>();

            var roleList = roles.ToList();

            var roleManager = new RoleManager<IdentityRole<int>>(
                store.Object,
                null,
                null,
                null,
                null
            );

            // Vi kan inte sätta Roles via ctor, men i testerna mockar vi vanligtvis direkt via Moq.
            // Här använder vi ett litet trick: skapa en Mock<RoleManager<>> där vi sätter Roles.

            var mockManager = new Mock<RoleManager<IdentityRole<int>>>(
                store.Object,
                null, null, null, null);

            mockManager.SetupGet(rm => rm.Roles)
                       .Returns(roleList.AsQueryable());

            mockManager
                .Setup(rm => rm.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string name) =>
                    roleList.FirstOrDefault(r => r.Name == name));

            return mockManager.Object;
        }

        public static Mock<IUserService> CreateUserServiceMock(UserUpdateDto? dto = null)
        {
            var mock = new Mock<IUserService>();

            if (dto != null)
            {
                mock.Setup(s => s.GetUser(dto.Id))
                    .Returns(dto);

                mock.Setup(s => s.Update(It.IsAny<UserUpdateDto>()))
                    .Callback<UserUpdateDto>(u => dto.IsDeleted = u.IsDeleted);
            }

            return mock;
        }
    }
}

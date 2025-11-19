using DAL.DTOs;
using DAL.Entities;
using MainApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Pages.Admin;

namespace Tests
{//tests
    public class UserDetailsModelTests
    {
        [Fact]
        public async Task OnGetAsync_Loads_User_When_Exists()
        {
            var db = TestHelpers.CreateDbContext(nameof(OnGetAsync_Loads_User_When_Exists));
            var user = new User
            {
                Id = 1,
                FirstName = "Member",
                LastName = "User",
                Email = "m@test.com",
                UserName = "member1"
            };
            var role = new IdentityRole<int> { Id = 10, Name = "Member" };

            db.Users.Add(user);
            db.Roles.Add(role);
            db.UserRoles.Add(new IdentityUserRole<int> { UserId = user.Id, RoleId = role.Id });
            await db.SaveChangesAsync();

            var roleManager = TestHelpers.CreateRoleManager(new[] { role });
            var userServiceMock = TestHelpers.CreateUserServiceMock();

            var model = new UserDetailsModel(db, roleManager, userServiceMock.Object);

            var result = await model.OnGetAsync(1, CancellationToken.None);

            Assert.IsType<PageResult>(result);
            Assert.NotNull(model.User);
            Assert.Equal("Member", model.User.FirstName);
        }

        [Fact]
        public async Task OnPostLockAsync_Sets_LockoutEnd()
        {
            var db = TestHelpers.CreateDbContext(nameof(OnPostLockAsync_Sets_LockoutEnd));
            var user = new User { Id = 1, UserName = "u1" };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var role = new IdentityRole<int> { Id = 10, Name = "Member" };
            var roleManager = TestHelpers.CreateRoleManager(new[] { role });
            var userServiceMock = TestHelpers.CreateUserServiceMock();

            var model = new UserDetailsModel(db, roleManager, userServiceMock.Object);

            var result = await model.OnPostLockAsync(1, CancellationToken.None);

            Assert.IsType<RedirectToPageResult>(result);
            var updated = await db.Users.FindAsync(1);
            Assert.NotNull(updated!.LockoutEnd);
        }

        [Fact]
        public async Task OnPostUnlockAsync_Clears_LockoutEnd()
        {
            var db = TestHelpers.CreateDbContext(nameof(OnPostUnlockAsync_Clears_LockoutEnd));
            var user = new User { Id = 1, UserName = "u1", LockoutEnd = DateTimeOffset.UtcNow.AddDays(1) };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var role = new IdentityRole<int> { Id = 10, Name = "Member" };
            var roleManager = TestHelpers.CreateRoleManager(new[] { role });
            var userServiceMock = TestHelpers.CreateUserServiceMock();

            var model = new UserDetailsModel(db, roleManager, userServiceMock.Object);

            var result = await model.OnPostUnlockAsync(1, CancellationToken.None);

            Assert.IsType<RedirectToPageResult>(result);
            var updated = await db.Users.FindAsync(1);
            Assert.Null(updated!.LockoutEnd);
        }

        [Fact]
        public void OnPostDelete_SoftDeletes_User_Via_UserService()
        {
            var dto = new UserUpdateDto { Id = 1, IsDeleted = false };

            var db = TestHelpers.CreateDbContext(nameof(OnPostDelete_SoftDeletes_User_Via_UserService));
            var role = new IdentityRole<int> { Id = 10, Name = "Member" };
            var roleManager = TestHelpers.CreateRoleManager(new[] { role });
            var userServiceMock = TestHelpers.CreateUserServiceMock(dto);

            var model = new UserDetailsModel(db, roleManager, userServiceMock.Object)
            {
                UserViewModel = new UserViewModel { Id = 1 }
            };

            var result = model.OnPostDelete(1);

            Assert.IsType<RedirectToPageResult>(result);
            Assert.True(dto.IsDeleted);
        }
    }
}

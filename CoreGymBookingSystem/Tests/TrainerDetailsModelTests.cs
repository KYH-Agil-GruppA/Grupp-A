using DAL.DTOs;
using DAL.Entities;
using MainApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tests;
using WebApp.Pages.Admin;

namespace Tests
{
    public class TrainerDetailsModelTests
    {


        [Fact]
        public async Task OnPostSaveAsync_Updates_Trainer_Fields()
        {
            var db = TestHelpers.CreateDbContext(nameof(OnPostSaveAsync_Updates_Trainer_Fields));
            var trainer = new User
            {
                Id = 1,
                FirstName = "Old",
                LastName = "Trainer",
                Email = "old@test.com",
                Address = "Old addr",
                City = "Old city",
                Country = "Old country"
            };
            db.Users.Add(trainer);
            await db.SaveChangesAsync();

            var role = new IdentityRole<int> { Id = 10, Name = "Trainer" };
            var roleManager = TestHelpers.CreateRoleManager(new[] { role });
            var userServiceMock = TestHelpers.CreateUserServiceMock();

            var model = new TrainerDetailsModel(db, userServiceMock.Object, roleManager)
            {
                Input = new TrainerDetailsModel.EditTrainerInput
                {
                    Id = 1,
                    FirstName = "New",
                    LastName = "Trainer",
                    Email = "new@test.com",
                    Address = "New addr",
                    City = "New city",
                    Country = "New country"
                }
            };

            var result = await model.OnPostSaveAsync(CancellationToken.None);

            Assert.IsType<RedirectToPageResult>(result);

            var updated = await db.Users.FindAsync(1);
            Assert.Equal("New", updated!.FirstName);
            Assert.Equal("new@test.com", updated.Email);
        }

        [Fact]
        public void OnPostDelete_SoftDeletes_Trainer_Via_UserService()
        {
            var dto = new UserUpdateDto { Id = 1, IsDeleted = false };

            var db = TestHelpers.CreateDbContext(nameof(OnPostDelete_SoftDeletes_Trainer_Via_UserService));
            var role = new IdentityRole<int> { Id = 10, Name = "Trainer" };
            var roleManager = TestHelpers.CreateRoleManager(new[] { role });
            var userServiceMock = TestHelpers.CreateUserServiceMock(dto);

            var model = new TrainerDetailsModel(db, userServiceMock.Object, roleManager)
            {
                UserViewModel = new UserViewModel { Id = 1 }
            };

            var result = model.OnPostDelete(1);

            Assert.IsType<RedirectToPageResult>(result);
            Assert.True(dto.IsDeleted);
        }
    }
}

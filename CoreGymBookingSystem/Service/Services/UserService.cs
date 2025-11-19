using DAL.DbContext;
using DAL.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }



       
        public Task DeleteUser(UserUpdateDto user)
        {
            var userToDelete = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            if (userToDelete != null)
            {
                
                userToDelete.IsDeleted = user.IsDeleted;
                user.SessionDeletes.ForEach(sessionDto =>
                {
                    var session = _dbContext.Sessions.FirstOrDefault(s => s.Id == sessionDto.Id);
                    if (session != null)
                    {
                        session.IsDeleted = sessionDto.IsDeleted;
                    }
                });
                _dbContext.SaveChanges();
            }
            return Task.CompletedTask;


        }

        public UserUpdateDto GetUser(int id)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                return new UserUpdateDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    City = user.City,
                    Country = user.Country,
                    IsDeleted = user.IsDeleted
                };
            }
            return null;


        }

        public void Update(UserUpdateDto user)
        {
            var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
            if (userToUpdate != null)
            {
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Address = user.Address;
                userToUpdate.City = user.City;
                userToUpdate.Country = user.Country;
                userToUpdate.IsDeleted = user.IsDeleted;
                _dbContext.SaveChanges();
            }


        }
    }
}

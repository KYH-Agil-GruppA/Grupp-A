using DAL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {

        UserUpdateDto GetUser(int id);
        Task DeleteUser(UserUpdateDto user);

        void Update(UserUpdateDto user);
    }
}

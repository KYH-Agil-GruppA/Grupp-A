using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMembershipRepository
    {
        Task<List<MembershipType>> GetAllAsync();
        Task<MembershipType?> GetByIdAsync(int id);
    }

}

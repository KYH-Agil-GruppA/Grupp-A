using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IMembershipService
    {
        Task<List<MembershipType>> GetAllMemberships();
        Task<MembershipType?> GetMembership(int id);
    }

}

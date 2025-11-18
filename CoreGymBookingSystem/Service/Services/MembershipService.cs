using DAL.Entities;
using DAL.Repositories.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _repo;

        public MembershipService(IMembershipRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<MembershipType>> GetAllMemberships()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<MembershipType?> GetMembership(int id)
        {
            return await _repo.GetByIdAsync(id);
        }
    }

}

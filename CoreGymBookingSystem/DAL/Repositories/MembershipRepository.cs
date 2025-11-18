using DAL.DbContext;
using DAL.Entities;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly ApplicationDbContext _context;

        public MembershipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MembershipType>> GetAllAsync()
        {
            return await _context.MembershipTypes.ToListAsync();
        }

        public async Task<MembershipType?> GetByIdAsync(int id)
        {
            return await _context.MembershipTypes.FindAsync(id);
        }
    }

}

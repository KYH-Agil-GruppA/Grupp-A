using DAL.DbContext;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MembershipPurchaseService : IMembershipPurchaseService
    {
        private readonly ApplicationDbContext _context;

        public MembershipPurchaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MembershipPurchase purchase)
        {
            _context.MembershipPurchases.Add(purchase);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string email, DateOnly startDate)
        {
            return await _context.MembershipPurchases
                .AnyAsync(x => x.Email == email && x.StartDate == startDate);
        }


    }
}

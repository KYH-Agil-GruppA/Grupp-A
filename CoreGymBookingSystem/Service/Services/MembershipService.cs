using DAL.DbContext;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;

namespace Service.Services;

public class MembershipService : IMembershipService
{
    private readonly ApplicationDbContext _context;

    public MembershipService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MembershipType>> GetAllAsync()
    {
        return await _context.MembershipTypes.ToListAsync();
    }

    public async Task<MembershipType?> GetByIdAsync(int id)
    {
        return await _context.MembershipTypes
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}



using DAL.Entities;

namespace Service.Interfaces;

public interface IMembershipService
{
    Task<List<MembershipType>> GetAllAsync();
    Task<MembershipType?> GetByIdAsync(int id);
}


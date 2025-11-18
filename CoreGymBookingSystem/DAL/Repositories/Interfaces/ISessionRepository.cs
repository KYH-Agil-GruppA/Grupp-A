using DAL.Entities;

namespace DAL.Repositories.Interfaces;

public interface ISessionRepository
{
    Task<List<Session>> GetAllAsync();
    Task<Session?> GetByIdAsync(int id);
  
    Task AddAsync(Session session);
    Task SaveChangesAsync();
   
    void AttachUserById(int id);
    Task AddAsyncWithInstructor(Session entity, int instructorId);
    Task<List<Session>> GetByInstructorWithDetailsAsync(int instructorId, DateTime weekStart, DateTime weekEnd);
    Task<bool> HasOverlapAsync(int instructorId, DateTime start, DateTime end, int? excludeSessionId = null);

}

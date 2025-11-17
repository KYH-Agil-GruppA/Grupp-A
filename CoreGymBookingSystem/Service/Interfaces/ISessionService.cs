using DAL.Entities;
using DAL.DTOs;

namespace Services.Interfaces
{
    public interface ISessionService
    {
        Task<List<Session>> GetAllSessionsAsync();
        Task<Session?> GetSessionByIdAsync(int id);
        Task<List<SessionsDto>> SearchByCategory(string category);
        Task<List<SessionsDto>> GetSessionsByCategoryAsync(string category);
        Task<List<SessionsDto>> GetDetailedForInstructorWeekAsync(int instructorId, DateTime weekStart);
        Task CreateAsync(SessionCreateDto dto);


    }
}

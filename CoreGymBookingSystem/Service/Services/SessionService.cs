using DAL.DTOs;
using DAL.Entities;
using DAL.Repositories.Interfaces;
using Services.Interfaces;

namespace Service.Services;

public class SessionService : ISessionService
{
    private static readonly HashSet<string> AllowedCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        "Yoga", "Running", "Weightloss", "Cardio", "Bodybuilding", "Nutrition"
    };

    private readonly ISessionRepository _sessionRepository;

    public SessionService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }


    public async Task<List<Session>> GetAllSessionsAsync()
    {
        return await _sessionRepository.GetAllAsync();
    }

    public async Task<Session?> GetSessionByIdAsync(int id)
    {
        return await _sessionRepository.GetByIdAsync(id);
    }

    public async Task<List<SessionsDto>> GetDetailedForInstructorWeekAsync(int instructorId, DateTime weekStart)
    {
        var weekEnd = weekStart.Date.AddDays(7);

        var entities = await _sessionRepository.GetByInstructorWithDetailsAsync(
            instructorId, weekStart, weekEnd);

        return entities
            .Select(s => new SessionsDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                Category = s.Category,
                DayOfWeek = s.StartTime.DayOfWeek.ToString(),
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                InstructorUserName = s.Instructor?.UserName,
                MaxParticipants = s.MaxParticipants,
                CurrentBookings = s.Bookings?.Count ?? 0
            })
            .ToList();
    }

    public async Task<List<SessionsDto>> SearchByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return new List<SessionsDto>();

        var sessions = await _sessionRepository.GetAllAsync();

        var filteredSessions = sessions
            .Where(s => s.Category != null &&
                        s.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .Select(s => new SessionsDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                Category = s.Category,
                DayOfWeek = s.StartTime.DayOfWeek.ToString(),
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                InstructorUserName = s.Instructor?.UserName,
                MaxParticipants = s.MaxParticipants,
                CurrentBookings = s.Bookings?.Count ?? 0
            })
            .ToList();

        return filteredSessions;
    }

    public async Task<List<SessionsDto>> GetSessionsByCategoryAsync(string category)
    {
        if (!AllowedCategories.Contains(category))
            return new List<SessionsDto>();

        var sessions = await _sessionRepository.GetAllAsync();

        return sessions
            .Where(s => !string.IsNullOrEmpty(s.Category) &&
                        s.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .Select(s => new SessionsDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                Category = s.Category,
                DayOfWeek = s.StartTime.DayOfWeek.ToString(),
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                InstructorUserName = s.Instructor?.UserName,
                MaxParticipants = s.MaxParticipants,
                CurrentBookings = s.Bookings?.Count ?? 0
            })
            .ToList();
    }


    public async Task CreateAsync(SessionCreateDto dto)
    {
        if (dto.EndTime <= dto.StartTime)
            throw new ArgumentException("End time must be after start time.");

        var categoryString = dto.Category.ToString();

        if (!AllowedCategories.Contains(categoryString))
            throw new ArgumentException($"Category '{dto.Category}' is not allowed.");

        bool hasOverlap = await _sessionRepository.HasOverlapAsync(
            dto.InstructorId, dto.StartTime, dto.EndTime, excludeSessionId: null);

        if (hasOverlap)
            throw new InvalidOperationException("You already have a class scheduled during this time.");

        var session = new Session
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = categoryString,
            MaxParticipants = dto.MaxParticipants,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        await _sessionRepository.AddAsyncWithInstructor(session, dto.InstructorId);
        await _sessionRepository.SaveChangesAsync();
    }
}

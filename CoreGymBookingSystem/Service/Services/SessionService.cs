using DAL.DTOs;
using DAL.Entities;
using DAL.Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class SessionService : ISessionService
    {
        private static readonly HashSet<string> Allowed =
      new HashSet<string>(StringComparer.OrdinalIgnoreCase)
      {
            "Yoga","Running","Weightloss","Cardio","Bodybuilding","Nutrition"
      };

        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }



        public async Task<List<SessionsDto>> GetDetailedForInstructorWeekAsync(int instructorId, DateTime weekStart)
        {
            DateTime weekEnd = weekStart.Date.AddDays(7);

            var entities = await _sessionRepository.GetByInstructorWithDetailsAsync(instructorId, weekStart, weekEnd);

            var result = new List<SessionsDto>();
            foreach (var s in entities)
            {
                var dto = new SessionsDto();
                dto.Id = s.Id;
                dto.Title = s.Title;
                dto.Description = s.Description;
                dto.Category = s.Category;
                dto.DayOfWeek = s.StartTime.DayOfWeek.ToString();
                dto.StartTime = s.StartTime;
                dto.EndTime = s.EndTime;
                dto.InstructorUserName = s.Instructor != null ? s.Instructor.UserName : null;
                dto.MaxParticipants = s.MaxParticipants;
                dto.CurrentBookings = s.Bookings != null ? s.Bookings.Count : 0;
                result.Add(dto);
            }

            return result;
        }


        public async Task<List<Session>> GetAllSessionsAsync()
        {
            return await _sessionRepository.GetAllAsync();
        }

        public async Task<Session?> GetSessionByIdAsync(int id)
        {
            return await _sessionRepository.GetByIdAsync(id);
        }

        public async Task<List<SessionsDto>> SearchByCategory(string category)
        {
            var sessions = await  _sessionRepository.GetAllAsync();
            var filteredSessions = sessions
                .Where(s => s.Category != null && s.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SessionsDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    Category = s.Category,
                   
                })
                .ToList();

            return filteredSessions;

        }
        public async Task<List<SessionsDto>> GetSessionsByCategoryAsync(string category)
        {
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
                    CurrentBookings = s.Bookings.Count
                })
                .ToList();
        }
        public async Task CreateAsync(SessionCreateDto dto)
        {
            if (dto.EndTime <= dto.StartTime)
            {
                throw new ArgumentException("End time must be after start time.");
            }

         
            bool hasClash = await _sessionRepository.HasOverlapAsync(dto.InstructorId, dto.StartTime, dto.EndTime, null);
            if (hasClash)
            {
                throw new InvalidOperationException("You already have a class scheduled during this time.");
            }

        
            var entity = new Session();
            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.Category = dto.Category.ToString();
            entity.MaxParticipants = dto.MaxParticipants;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;

        
            await _sessionRepository.AddAsyncWithInstructor(entity, dto.InstructorId);
            await _sessionRepository.SaveChangesAsync();
        }
}
}





using DAL.DbContext;
using DAL.Entities;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Session>> GetAllAsync()
    {
        return await _context.Sessions
            .Include(s => s.Bookings)
            .Include(s => s.Instructor)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Session?> GetByIdAsync(int id)
    {
        return await _context.Sessions
            .Include(s => s.Bookings)
            .Include(s => s.Instructor)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddAsync(Session entity)
    {
        await _context.Sessions.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void AttachUserById(int id)
    {
        _context.Attach(new User { Id = id });
    }

    /// <summary>
    /// Adds a new session and sets the Instructor via the real foreign key property.
    /// No need for shadow properties anymore – EF Core generates InstructorId automatically.
    /// </summary>
    public async Task AddAsyncWithInstructor(Session entity, int instructorId)
    {
        entity.InstructorId = instructorId;   // Direct assignement, EF Core exposes the FK property in queries and change tracking
        entity.Instructor = null;             // Prevents accidental loading of the full User entity

        await _context.Sessions.AddAsync(entity);
    }

    /// <summary>
    /// Returns all sessions for an instructor in a given week, with related data
    /// </summary>
    public async Task<List<Session>> GetByInstructorWithDetailsAsync(int instructorId, DateTime weekStart, DateTime weekEnd)
    {
        return await _context.Sessions
            .Where(s => s.InstructorId == instructorId &&
                        s.StartTime >= weekStart &&
                        s.StartTime < weekEnd)
            .Include(s => s.Instructor)
            .Include(s => s.Bookings)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    /// <summary>
    /// Checks for time overlap with other sessions for the same instructor
    /// </summary>
    public async Task<bool> HasOverlapAsync(int instructorId, DateTime start, DateTime end, int? excludeSessionId = null)
    {
        var query = _context.Sessions
            .Where(s => s.InstructorId == instructorId &&
                        s.StartTime < end &&
                        s.EndTime > start);

        if (excludeSessionId.HasValue)
            query = query.Where(s => s.Id != excludeSessionId.Value);

        return await query.AnyAsync();
    }
}
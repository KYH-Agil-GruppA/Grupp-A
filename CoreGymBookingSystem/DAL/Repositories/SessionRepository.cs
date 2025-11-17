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


    // NEW: check if there is any overlapping session for the same instructor
    public async Task<bool> HasOverlapAsync(int instructorId, DateTime start, DateTime end, int? excludeSessionId = null)
    {
        // Overlap condition: existing.Start < new.End AND existing.End > new.Start
        // Using query syntax and no lambda predicates in Any().
        var query =
            from s in _context.Sessions
            where EF.Property<int>(s, "InstructorId") == instructorId
                  && s.StartTime < end
                  && s.EndTime > start
            select s.Id;

        if (excludeSessionId.HasValue)
        {
            query =
                from id in query
                where id != excludeSessionId.Value
                select id;
        }

        var list = await query.Take(1).ToListAsync(); // cheap existence check without Any(predicate)
        return list.Count > 0;
    }

    public async Task AddAsyncWithInstructor(Session entity, int instructorId)
    {
        // make sure no User entity is in the graph
        entity.Instructor = null;

        // set the shadow FK column "InstructorId"
        _context.Entry(entity).Property<int>("InstructorId").CurrentValue = instructorId;

        await _context.Sessions.AddAsync(entity);
    }

    public async Task AddAsync(Session entity)
    {
        await _context.Sessions.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IList<Session>> GetByInstructorAsync(int instructorId, DateTime weekStart, DateTime weekEnd)
    {
        // filter by the shadow FK to avoid an unnecessary join
        var query =
            from s in _context.Sessions
            where EF.Property<int>(s, "InstructorId") == instructorId
                  && s.StartTime >= weekStart
                  && s.StartTime < weekEnd
            orderby s.StartTime
            select s;

        return await query.ToListAsync();
    }

    public void AttachUserById(int id)
    {
        _context.Attach(new User { Id = id });
    }


    public async Task<List<Session>> GetAllAsync()
    {
        return await _context.Sessions.Include(s => s.Instructor).ToListAsync();
    }

    public async Task<Session?> GetByIdAsync(int id)
    {
        return await _context.Sessions
            .Include(s => s.Instructor)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Session>> GetByInstructorWithDetailsAsync(int instructorId, DateTime weekStart, DateTime weekEnd)
    {
        // shadow FK "InstructorId" (int) exists by convention
        var query =
            from s in _context.Sessions.Include("Instructor").Include("Bookings")
            where EF.Property<int>(s, "InstructorId") == instructorId
                  && s.StartTime >= weekStart
                  && s.StartTime < weekEnd
            orderby s.StartTime
            select s;

        return await query.ToListAsync();
    }

    public void SetInstructorId(Session entity, int instructorId)
    {
        _context.Entry(entity).Property<int>("InstructorId").CurrentValue = instructorId;
    }

    //public async Task AddAsync(Session session)
    //{
    //    await _context.Sessions.AddAsync(session);
    //}

    //public async Task SaveChangesAsync()
    //{
    //    await _context.SaveChangesAsync();
    //}

}

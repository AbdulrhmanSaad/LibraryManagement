using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.Infrastructure.Repositories;

public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
{
    public ActivityLogRepository(LibraryDbContext context) : base(context) { }

    public async Task<IEnumerable<ActivityLog>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(al => al.User)
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActivityLog>> GetRecentLogsAsync(int count)
    {
        return await _dbSet
            .Include(al => al.User)
            .OrderByDescending(al => al.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActivityLog>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _dbSet
            .Include(al => al.User)
            .Where(al => al.Timestamp >= from && al.Timestamp <= to)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }
}

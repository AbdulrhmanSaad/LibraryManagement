using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces;

public interface IActivityLogRepository : IGenericRepository<ActivityLog>
{
    Task<IEnumerable<ActivityLog>> GetByUserIdAsync(int userId);
    Task<IEnumerable<ActivityLog>> GetRecentLogsAsync(int count);
    Task<IEnumerable<ActivityLog>> GetByDateRangeAsync(DateTime from, DateTime to);
}

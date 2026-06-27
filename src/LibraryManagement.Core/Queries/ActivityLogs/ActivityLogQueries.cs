using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Queries.ActivityLogs;

public record GetRecentActivityLogsQuery(int Count = 50) : IRequest<IEnumerable<ActivityLogDto>>;

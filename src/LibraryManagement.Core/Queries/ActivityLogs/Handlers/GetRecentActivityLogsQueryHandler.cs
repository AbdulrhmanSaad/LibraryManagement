using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.ActivityLogs;

namespace LibraryManagement.Core.Queries.ActivityLogs.Handlers;

public class GetRecentActivityLogsQueryHandler : IRequestHandler<GetRecentActivityLogsQuery, IEnumerable<ActivityLogDto>>
{
    private readonly IActivityLogRepository _repository;
    private readonly IMapper _mapper;
    public GetRecentActivityLogsQueryHandler(IActivityLogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper; 
    }
    public async Task<IEnumerable<ActivityLogDto>> Handle(GetRecentActivityLogsQuery request, CancellationToken ct)
        => _mapper.Map<IEnumerable<ActivityLogDto>>(await _repository.GetRecentLogsAsync(request.Count));
}

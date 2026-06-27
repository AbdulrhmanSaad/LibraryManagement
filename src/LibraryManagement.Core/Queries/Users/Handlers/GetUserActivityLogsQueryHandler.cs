using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Queries.Users.Handlers;

public class GetUserActivityLogsQueryHandler : IRequestHandler<GetUserActivityLogsQuery, IEnumerable<ActivityLogDto>>
{
    private readonly IActivityLogRepository _repository;
    private readonly IMapper _mapper;
    public GetUserActivityLogsQueryHandler(IActivityLogRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<ActivityLogDto>> Handle(GetUserActivityLogsQuery request, CancellationToken ct)
        => _mapper.Map<IEnumerable<ActivityLogDto>>(await _repository.GetByUserIdAsync(request.UserId));
}

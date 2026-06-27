using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Members;

namespace LibraryManagement.Core.Queries.Members.Handlers;

public class GetAllMembersQueryHandler : IRequestHandler<GetAllMembersQuery, IEnumerable<MemberDto>>
{
    private readonly IGenericRepository<Member> _repository;
    private readonly IMapper _mapper;
    public GetAllMembersQueryHandler(IGenericRepository<Member> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<MemberDto>> Handle(GetAllMembersQuery request, CancellationToken ct)
        => _mapper.Map<IEnumerable<MemberDto>>(await _repository.GetAllAsync());
}

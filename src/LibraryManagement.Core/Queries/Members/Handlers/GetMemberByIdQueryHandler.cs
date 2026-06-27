using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Members;

namespace LibraryManagement.Core.Queries.Members.Handlers;

public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto?>
{
    private readonly IGenericRepository<Member> _repository;
    private readonly IMapper _mapper;
    public GetMemberByIdQueryHandler(IGenericRepository<Member> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<MemberDto?> Handle(GetMemberByIdQuery request, CancellationToken ct)
    {
        var member = await _repository.GetByIdAsync(request.Id);
        return member == null ? null : _mapper.Map<MemberDto>(member);
    }
}

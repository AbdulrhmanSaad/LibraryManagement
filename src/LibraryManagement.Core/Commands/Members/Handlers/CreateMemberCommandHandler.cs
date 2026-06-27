using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Members;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Members.Handlers;

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, MemberDto>
{
    private readonly IGenericRepository<Member> _repository;
    private readonly IMapper _mapper;
    public CreateMemberCommandHandler(IGenericRepository<Member> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }

    public async Task<MemberDto> Handle(CreateMemberCommand request, CancellationToken ct)
    {
        var existing = await _repository.FindAsync(m => m.Email == request.Member.Email);
        if (existing.Any()) throw new InvalidOperationException("A member with this email already exists");

        var member = _mapper.Map<Member>(request.Member);
        member.MemberNumber = $"MEM-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        member.MembershipDate = DateTime.UtcNow;
        await _repository.AddAsync(member);
        return _mapper.Map<MemberDto>(member);
    }
}

using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Members;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Members.Handlers;

public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand>
{
    private readonly IGenericRepository<Member> _repository;
    public DeleteMemberCommandHandler(IGenericRepository<Member> repository) { _repository = repository; }

    public async Task Handle(DeleteMemberCommand request, CancellationToken ct)
    {
        var member = await _repository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Member not found");
        member.IsActive = false;
        await _repository.UpdateAsync(member);
    }
}

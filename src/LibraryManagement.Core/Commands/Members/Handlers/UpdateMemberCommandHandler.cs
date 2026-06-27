using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Members;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Members.Handlers;

public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand>
{
    private readonly IGenericRepository<Member> _repository;
    public UpdateMemberCommandHandler(IGenericRepository<Member> repository) { _repository = repository; }

    public async Task Handle(UpdateMemberCommand request, CancellationToken ct)
    {
        var member = await _repository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Member not found");
        var dto = request.Member;
        member.FirstName = dto.FirstName ?? member.FirstName;
        member.LastName = dto.LastName ?? member.LastName;
        member.Email = dto.Email ?? member.Email;
        member.Phone = dto.Phone ?? member.Phone;
        member.Address = dto.Address ?? member.Address;
        member.DateOfBirth = dto.DateOfBirth ?? member.DateOfBirth;
        member.IsActive = dto.IsActive ?? member.IsActive;
        await _repository.UpdateAsync(member);
    }
}

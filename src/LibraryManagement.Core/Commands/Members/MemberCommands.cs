using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Commands.Members;

public record CreateMemberCommand(CreateMemberDto Member) : IRequest<MemberDto>;
public record UpdateMemberCommand(int Id, UpdateMemberDto Member) : IRequest;
public record DeleteMemberCommand(int Id) : IRequest;

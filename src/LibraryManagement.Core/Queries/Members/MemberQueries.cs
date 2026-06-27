using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Queries.Members;

public record GetAllMembersQuery : IRequest<IEnumerable<MemberDto>>;
public record GetMemberByIdQuery(int Id) : IRequest<MemberDto?>;
public record GetMemberBorrowingsQuery(int MemberId) : IRequest<IEnumerable<BorrowingTransactionDto>>;

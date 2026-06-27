using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Queries.Borrowing;

public record GetAllBorrowingsQuery : IRequest<IEnumerable<BorrowingTransactionDto>>;
public record GetBorrowingByIdQuery(int Id) : IRequest<BorrowingTransactionDto?>;
public record GetActiveBorrowingsQuery : IRequest<IEnumerable<BorrowingTransactionDto>>;
public record GetBorrowingsByMemberQuery(int MemberId) : IRequest<IEnumerable<BorrowingTransactionDto>>;

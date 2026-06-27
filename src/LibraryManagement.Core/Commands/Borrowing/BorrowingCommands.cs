using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Commands.Borrowing;

public record BorrowBookCommand(CreateBorrowingDto Borrowing) : IRequest<BorrowingTransactionDto>;
public record ReturnBookCommand(ReturnBookDto Return) : IRequest<BorrowingTransactionDto>;

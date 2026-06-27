using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.Commands.Borrowing;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Borrowing.Handlers;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, BorrowingTransactionDto>
{
    private readonly IBorrowingRepository _borrowingRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IMapper _mapper;

    public ReturnBookCommandHandler(IBorrowingRepository borrowingRepo, IBookRepository bookRepo, IMapper mapper)
    {
        _borrowingRepo = borrowingRepo; _bookRepo = bookRepo; _mapper = mapper;
    }

    public async Task<BorrowingTransactionDto> Handle(ReturnBookCommand request, CancellationToken ct)
    {
        var transaction = await _borrowingRepo.GetTransactionWithDetailsAsync(request.Return.TransactionId)
            ?? throw new KeyNotFoundException("Transaction not found");

        if (transaction.Status == "Returned") throw new InvalidOperationException("Book already returned");

        var book = await _bookRepo.GetByIdAsync(transaction.BookId) ?? throw new KeyNotFoundException("Book not found");

        transaction.ReturnDate = DateTime.UtcNow;
        transaction.Status = "Returned";
        transaction.Notes = request.Return.Notes ?? transaction.Notes;

        book.Status = BookStatus.In;
        await _bookRepo.UpdateAsync(book);
        await _borrowingRepo.UpdateAsync(transaction);
        return _mapper.Map<BorrowingTransactionDto>(await _borrowingRepo.GetTransactionWithDetailsAsync(transaction.Id));
    }
}

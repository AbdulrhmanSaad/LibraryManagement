using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.Commands.Borrowing;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Borrowing.Handlers;

public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowingTransactionDto>
{
    private readonly IBorrowingRepository _borrowingRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IGenericRepository<Member> _memberRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public BorrowBookCommandHandler(IBorrowingRepository borrowingRepo, IBookRepository bookRepo, IGenericRepository<Member> memberRepo, UserManager<AppUser> userManager, IMapper mapper)
    {
        _borrowingRepo = borrowingRepo; _bookRepo = bookRepo; _memberRepo = memberRepo; _userManager = userManager; _mapper = mapper;
    }

    public async Task<BorrowingTransactionDto> Handle(BorrowBookCommand request, CancellationToken ct)
    {
        var dto = request.Borrowing;
        var book = await _bookRepo.GetByIdAsync(dto.BookId) ?? throw new KeyNotFoundException("Book not found");
        if (book.Status != BookStatus.In) throw new InvalidOperationException("Book is not available for borrowing");

        var member = await _memberRepo.GetByIdAsync(dto.MemberId) ?? throw new KeyNotFoundException("Member not found");
        if (!member.IsActive) throw new InvalidOperationException("Member account is inactive");

        var user = await _userManager.FindByIdAsync(dto.BorrowedById.ToString()) ?? throw new KeyNotFoundException("User not found");

        var transaction = new BorrowingTransaction
        {
            BookId = dto.BookId, MemberId = dto.MemberId, BorrowedById = dto.BorrowedById,
            BorrowDate = dto.BorrowDate ?? DateTime.UtcNow,
            DueDate = (dto.BorrowDate ?? DateTime.UtcNow).AddDays(dto.BorrowDays),
            Status = "Borrowed", Notes = dto.Notes
        };

        book.Status = BookStatus.Out;
        await _bookRepo.UpdateAsync(book);
        await _borrowingRepo.AddAsync(transaction);
        return _mapper.Map<BorrowingTransactionDto>(await _borrowingRepo.GetTransactionWithDetailsAsync(transaction.Id));
    }
}

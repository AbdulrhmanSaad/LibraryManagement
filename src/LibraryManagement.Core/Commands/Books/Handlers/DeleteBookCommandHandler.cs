using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Books;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Books.Handlers;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly IBookRepository _bookRepository;

    public DeleteBookCommandHandler(IBookRepository bookRepository) => _bookRepository = bookRepository;

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException("Book not found");

        book.Status = BookStatus.In;
        book.UpdatedAt = DateTime.UtcNow;
        await _bookRepository.UpdateAsync(book);
    }
}

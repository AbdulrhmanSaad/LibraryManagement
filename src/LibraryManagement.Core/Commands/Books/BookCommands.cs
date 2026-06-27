using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Commands.Books;

public record CreateBookCommand(CreateBookDto Book) : IRequest<BookDto>;

public record UpdateBookCommand(int Id, UpdateBookDto Book) : IRequest;

public record DeleteBookCommand(int Id) : IRequest;

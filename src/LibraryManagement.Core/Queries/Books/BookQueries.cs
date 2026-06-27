using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Queries.Books;

public record GetAllBooksQuery : IRequest<IEnumerable<BookDto>>;

public record GetBookByIdQuery(int Id) : IRequest<BookDto?>;

public record SearchBooksQuery(string? Title, string? Author, string? Category, string? Language, string? Status) : IRequest<IEnumerable<BookDto>>;

public record GetBooksByStatusQuery(BookStatus Status) : IRequest<IEnumerable<BookDto>>;

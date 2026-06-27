using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Books;

namespace LibraryManagement.Core.Queries.Books.Handlers;

public class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, IEnumerable<BookDto>>
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;
    public SearchBooksQueryHandler(IBookRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<BookDto>> Handle(SearchBooksQuery request, CancellationToken ct)
    {
        var books = await _repository.SearchBooksAsync(request.Title, request.Author, request.Category);
        if (!string.IsNullOrWhiteSpace(request.Language))
            books = books.Where(b => b.Language.Contains(request.Language));
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<BookStatus>(request.Status, true, out var parsedStatus))
            books = books.Where(b => b.Status == parsedStatus);
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }
}

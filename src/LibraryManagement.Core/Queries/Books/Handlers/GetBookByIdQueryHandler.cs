using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Books;

namespace LibraryManagement.Core.Queries.Books.Handlers;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;
    public GetBookByIdQueryHandler(IBookRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken ct)
    {
        var book = await _repository.GetBookWithDetailsAsync(request.Id);
        return book == null ? null : _mapper.Map<BookDto>(book);
    }
}

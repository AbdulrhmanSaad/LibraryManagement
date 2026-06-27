using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Books;

namespace LibraryManagement.Core.Queries.Books.Handlers;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDto>>
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;
    public GetAllBooksQueryHandler(IBookRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<BookDto>> Handle(GetAllBooksQuery request, CancellationToken ct)
        => _mapper.Map<IEnumerable<BookDto>>(await _repository.GetAllWithDetailsAsync());
}

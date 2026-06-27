using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Books;

namespace LibraryManagement.Core.Queries.Books.Handlers;

public class GetBooksByStatusQueryHandler : IRequestHandler<GetBooksByStatusQuery, IEnumerable<BookDto>>
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;
    public GetBooksByStatusQueryHandler(IBookRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<BookDto>> Handle(GetBooksByStatusQuery request, CancellationToken ct)
        => _mapper.Map<IEnumerable<BookDto>>(await _repository.GetBooksByStatusAsync(request.Status));
}

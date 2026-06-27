using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Authors;

namespace LibraryManagement.Core.Queries.Authors.Handlers;

public class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, IEnumerable<AuthorDto>>
{
    private readonly IGenericRepository<Author> _repository;
    private readonly IMapper _mapper;
    public GetAllAuthorsQueryHandler(IGenericRepository<Author> repository, IMapper mapper) 
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<IEnumerable<AuthorDto>> Handle(GetAllAuthorsQuery request, CancellationToken ct)
        => _mapper.Map<IEnumerable<AuthorDto>>(await _repository.GetAllAsync());
}

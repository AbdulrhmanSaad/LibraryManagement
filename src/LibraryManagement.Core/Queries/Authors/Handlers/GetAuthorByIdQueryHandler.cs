using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Authors;

namespace LibraryManagement.Core.Queries.Authors.Handlers;

public class GetAuthorByIdQueryHandler : IRequestHandler<GetAuthorByIdQuery, AuthorDto?>
{
    private readonly IGenericRepository<Author> _repository;
    private readonly IMapper _mapper;
    public GetAuthorByIdQueryHandler(IGenericRepository<Author> repository, IMapper mapper) 
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AuthorDto?> Handle(GetAuthorByIdQuery request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        return entity == null ? null : _mapper.Map<AuthorDto>(entity);
    }
}

using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Authors;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Authors.Handlers;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private readonly IGenericRepository<Author> _repository;
    private readonly IMapper _mapper;
    public CreateAuthorCommandHandler(IGenericRepository<Author> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken ct)
    {
        var entity = _mapper.Map<Author>(request.Author);
        await _repository.AddAsync(entity);
        return _mapper.Map<AuthorDto>(entity);
    }
}

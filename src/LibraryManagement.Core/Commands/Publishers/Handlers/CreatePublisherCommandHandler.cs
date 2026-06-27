using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Publishers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Publishers.Handlers;

public class CreatePublisherCommandHandler : IRequestHandler<CreatePublisherCommand, PublisherDto>
{
    private readonly IGenericRepository<Publisher> _repository;
    private readonly IMapper _mapper;
    public CreatePublisherCommandHandler(IGenericRepository<Publisher> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<PublisherDto> Handle(CreatePublisherCommand request, CancellationToken ct)
    {
        var entity = _mapper.Map<Publisher>(request.Publisher);
        await _repository.AddAsync(entity);
        return _mapper.Map<PublisherDto>(entity);
    }
}

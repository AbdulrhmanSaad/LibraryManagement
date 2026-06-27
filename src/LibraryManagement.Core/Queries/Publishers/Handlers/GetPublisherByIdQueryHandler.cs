using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Publishers;

namespace LibraryManagement.Core.Queries.Publishers.Handlers;

public class GetPublisherByIdQueryHandler : IRequestHandler<GetPublisherByIdQuery, PublisherDto?>
{
    private readonly IGenericRepository<Publisher> _repository;
    private readonly IMapper _mapper;
    public GetPublisherByIdQueryHandler(IGenericRepository<Publisher> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<PublisherDto?> Handle(GetPublisherByIdQuery request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        return entity == null ? null : _mapper.Map<PublisherDto>(entity);
    }
}

using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Publishers;

namespace LibraryManagement.Core.Queries.Publishers.Handlers;

public class GetAllPublishersQueryHandler : IRequestHandler<GetAllPublishersQuery, IEnumerable<PublisherDto>>
{
    private readonly IGenericRepository<Publisher> _repository;
    private readonly IMapper _mapper;
    public GetAllPublishersQueryHandler(IGenericRepository<Publisher> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<PublisherDto>> Handle(GetAllPublishersQuery request, CancellationToken ct)
        => _mapper.Map<IEnumerable<PublisherDto>>(await _repository.GetAllAsync());
}

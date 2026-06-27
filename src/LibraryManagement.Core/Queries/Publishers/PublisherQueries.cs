using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Queries.Publishers;

public record GetAllPublishersQuery : IRequest<IEnumerable<PublisherDto>>;
public record GetPublisherByIdQuery(int Id) : IRequest<PublisherDto?>;

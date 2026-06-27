using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Queries.Authors;

public record GetAllAuthorsQuery : IRequest<IEnumerable<AuthorDto>>;
public record GetAuthorByIdQuery(int Id) : IRequest<AuthorDto?>;

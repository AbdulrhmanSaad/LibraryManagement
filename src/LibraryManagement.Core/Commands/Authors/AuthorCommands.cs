using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Commands.Authors;

public record CreateAuthorCommand(CreateAuthorDto Author) : IRequest<AuthorDto>;
public record UpdateAuthorCommand(int Id, UpdateAuthorDto Author) : IRequest;
public record DeleteAuthorCommand(int Id) : IRequest;

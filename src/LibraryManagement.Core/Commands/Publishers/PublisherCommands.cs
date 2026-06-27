using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Commands.Publishers;

public record CreatePublisherCommand(CreatePublisherDto Publisher) : IRequest<PublisherDto>;
public record UpdatePublisherCommand(int Id, UpdatePublisherDto Publisher) : IRequest;
public record DeletePublisherCommand(int Id) : IRequest;

using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Publishers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Publishers.Handlers;

public class DeletePublisherCommandHandler : IRequestHandler<DeletePublisherCommand>
{
    private readonly IGenericRepository<Publisher> _repository;
    public DeletePublisherCommandHandler(IGenericRepository<Publisher> repository) { _repository = repository; }
    public async Task Handle(DeletePublisherCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Publisher not found");
        await _repository.DeleteAsync(entity);
    }
}

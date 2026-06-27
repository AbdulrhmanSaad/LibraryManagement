using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Authors;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Authors.Handlers;

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand>
{
    private readonly IGenericRepository<Author> _repository;
    public DeleteAuthorCommandHandler(IGenericRepository<Author> repository) { _repository = repository; }
    public async Task Handle(DeleteAuthorCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Author not found");
        await _repository.DeleteAsync(entity);
    }
}

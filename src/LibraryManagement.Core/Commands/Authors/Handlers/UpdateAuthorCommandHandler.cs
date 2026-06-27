using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Authors;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Authors.Handlers;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand>
{
    private readonly IGenericRepository<Author> _repository;
    public UpdateAuthorCommandHandler(IGenericRepository<Author> repository) { _repository = repository; }
    public async Task Handle(UpdateAuthorCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Author not found");
        var dto = request.Author;
        if (dto.FirstName != null) entity.FirstName = dto.FirstName;
        if (dto.LastName != null) entity.LastName = dto.LastName;
        if (dto.Biography != null) entity.Biography = dto.Biography;
        if (dto.BirthDate.HasValue) entity.BirthDate = dto.BirthDate;
        await _repository.UpdateAsync(entity);
    }
}

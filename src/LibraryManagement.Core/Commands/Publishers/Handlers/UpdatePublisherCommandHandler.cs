using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Publishers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Publishers.Handlers;

public class UpdatePublisherCommandHandler : IRequestHandler<UpdatePublisherCommand>
{
    private readonly IGenericRepository<Publisher> _repository;
    public UpdatePublisherCommandHandler(IGenericRepository<Publisher> repository) { _repository = repository; }
    public async Task Handle(UpdatePublisherCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Publisher not found");
        var dto = request.Publisher;
        entity.Name = dto.Name ?? entity.Name;
        entity.Address = dto.Address ?? entity.Address;
        entity.Phone = dto.Phone ?? entity.Phone;
        entity.Email = dto.Email ?? entity.Email;
        entity.Website = dto.Website ?? entity.Website;
        await _repository.UpdateAsync(entity);
    }
}

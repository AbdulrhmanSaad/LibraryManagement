using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Categories;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Categories.Handlers;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IGenericRepository<Category> _repository;
    public DeleteCategoryCommandHandler(IGenericRepository<Category> repository) { _repository = repository; }
    public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Category not found");
        await _repository.DeleteAsync(entity);
    }
}

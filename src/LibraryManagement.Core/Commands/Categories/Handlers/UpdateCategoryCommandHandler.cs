using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Categories;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Categories.Handlers;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IGenericRepository<Category> _repository;
    public UpdateCategoryCommandHandler(IGenericRepository<Category> repository) { _repository = repository; }
    public async Task Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Category not found");
        var dto = request.Category;

        entity.Name = dto.Name ?? entity.Name;
        entity.Description = dto.Description ?? entity.Description;
        entity.ParentCategoryId = dto.ParentCategoryId ?? entity.ParentCategoryId;
        await _repository.UpdateAsync(entity);
    }
}

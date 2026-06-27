using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Categories;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Categories.Handlers;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IGenericRepository<Category> _repository;
    private readonly IMapper _mapper;
    public CreateCategoryCommandHandler(IGenericRepository<Category> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var entity = _mapper.Map<Category>(request.Category);
        await _repository.AddAsync(entity);
        return _mapper.Map<CategoryDto>(entity);
    }
}

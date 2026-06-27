using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Categories;

namespace LibraryManagement.Core.Queries.Categories.Handlers;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly IGenericRepository<Category> _repository;
    private readonly IMapper _mapper;
    public GetCategoryByIdQueryHandler(IGenericRepository<Category> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        return entity == null ? null : _mapper.Map<CategoryDto>(entity);
    }
}

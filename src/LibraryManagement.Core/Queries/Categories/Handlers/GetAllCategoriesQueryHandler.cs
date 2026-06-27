using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Categories;

namespace LibraryManagement.Core.Queries.Categories.Handlers;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly IGenericRepository<Category> _repository;
    private readonly IMapper _mapper;
    public GetAllCategoriesQueryHandler(IGenericRepository<Category> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken ct)
    {
        var categories = await _repository.GetAllAsync();
        var dtos = _mapper.Map<List<CategoryDto>>(categories);
        foreach (var dto in dtos) dto.SubCategories = dtos.Where(c => c.ParentCategoryId == dto.Id).ToList();
        return dtos.Where(c => c.ParentCategoryId == null).ToList();
    }
}

using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Queries.Categories;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;
public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;

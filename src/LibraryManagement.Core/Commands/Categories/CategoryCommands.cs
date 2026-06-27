using MediatR;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Commands.Categories;

public record CreateCategoryCommand(CreateCategoryDto Category) : IRequest<CategoryDto>;
public record UpdateCategoryCommand(int Id, UpdateCategoryDto Category) : IRequest;
public record DeleteCategoryCommand(int Id) : IRequest;

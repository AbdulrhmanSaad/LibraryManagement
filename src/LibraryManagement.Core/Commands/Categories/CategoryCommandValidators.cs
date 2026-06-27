using FluentValidation;
using LibraryManagement.Core.Commands.Categories;

namespace LibraryManagement.Core.Commands.Categories;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Category.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category.Description).MaximumLength(500);
    }
}

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Category.Name).MaximumLength(200).When(x => x.Category.Name != null);
    }
}

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

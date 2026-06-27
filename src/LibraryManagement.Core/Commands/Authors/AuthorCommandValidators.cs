using FluentValidation;
using LibraryManagement.Core.Commands.Authors;

namespace LibraryManagement.Core.Commands.Authors;

public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(x => x.Author.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Author.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Author.Biography).MaximumLength(4000);
    }
}

public class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Author.FirstName).MaximumLength(100).When(x => x.Author.FirstName != null);
        RuleFor(x => x.Author.LastName).MaximumLength(100).When(x => x.Author.LastName != null);
        RuleFor(x => x.Author.Biography).MaximumLength(4000);
    }
}

public class DeleteAuthorCommandValidator : AbstractValidator<DeleteAuthorCommand>
{
    public DeleteAuthorCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

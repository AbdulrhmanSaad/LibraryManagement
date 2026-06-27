using FluentValidation;
using LibraryManagement.Core.Commands.Publishers;

namespace LibraryManagement.Core.Commands.Publishers;

public class CreatePublisherCommandValidator : AbstractValidator<CreatePublisherCommand>
{
    public CreatePublisherCommandValidator()
    {
        RuleFor(x => x.Publisher.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Publisher.Email).EmailAddress().MaximumLength(255).When(x => x.Publisher.Email != null);
        RuleFor(x => x.Publisher.Website).MaximumLength(255);
    }
}

public class UpdatePublisherCommandValidator : AbstractValidator<UpdatePublisherCommand>
{
    public UpdatePublisherCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Publisher.Name).MaximumLength(200).When(x => x.Publisher.Name != null);
    }
}

public class DeletePublisherCommandValidator : AbstractValidator<DeletePublisherCommand>
{
    public DeletePublisherCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

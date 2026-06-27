using FluentValidation;
using LibraryManagement.Core.Commands.Members;

namespace LibraryManagement.Core.Commands.Members;

public class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberCommandValidator()
    {
        RuleFor(x => x.Member.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Member.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Member.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Member.Phone).MaximumLength(50);
        RuleFor(x => x.Member.Address).MaximumLength(500);
    }
}

public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
    public UpdateMemberCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Member.FirstName).MaximumLength(100).When(x => x.Member.FirstName != null);
        RuleFor(x => x.Member.LastName).MaximumLength(100).When(x => x.Member.LastName != null);
        RuleFor(x => x.Member.Email).EmailAddress().MaximumLength(255).When(x => x.Member.Email != null);
    }
}

public class DeleteMemberCommandValidator : AbstractValidator<DeleteMemberCommand>
{
    public DeleteMemberCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

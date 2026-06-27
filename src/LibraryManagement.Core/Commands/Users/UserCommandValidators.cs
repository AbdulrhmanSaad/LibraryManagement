using FluentValidation;
using LibraryManagement.Core.Commands.Users;

namespace LibraryManagement.Core.Commands.Users;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.User.Username).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.User.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.User.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.User.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.User.Role).NotEmpty().Must(r => r is "Administrator" or "Librarian" or "Staff")
            .WithMessage("Role must be Administrator, Librarian, or Staff");
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.User.Email).EmailAddress().MaximumLength(255).When(x => x.User.Email != null);
        RuleFor(x => x.User.FullName).MaximumLength(200).When(x => x.User.FullName != null);
        RuleFor(x => x.User.Password).MinimumLength(6).MaximumLength(100).When(x => x.User.Password != null);
    }
}

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

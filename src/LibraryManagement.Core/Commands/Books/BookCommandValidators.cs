using FluentValidation;
using LibraryManagement.Core.Commands.Books;

namespace LibraryManagement.Core.Commands.Books;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Book.ISBN).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Book.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Book.Language).MaximumLength(50);
        RuleFor(x => x.Book.PublicationYear).InclusiveBetween(1000, 2100).When(x => x.Book.PublicationYear.HasValue);
        RuleFor(x => x.Book.PageCount).GreaterThan(0).When(x => x.Book.PageCount.HasValue);
    }
}

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Book.ISBN).MaximumLength(20).When(x => x.Book.ISBN != null);
        RuleFor(x => x.Book.Title).MaximumLength(500).When(x => x.Book.Title != null);
        RuleFor(x => x.Book.Language).MaximumLength(50).When(x => x.Book.Language != null);
        RuleFor(x => x.Book.PublicationYear).InclusiveBetween(1000, 2100).When(x => x.Book.PublicationYear.HasValue);
        RuleFor(x => x.Book.PageCount).GreaterThan(0).When(x => x.Book.PageCount.HasValue);
    }
}

public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

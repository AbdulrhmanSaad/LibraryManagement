using FluentValidation;
using LibraryManagement.Core.Commands.Borrowing;

namespace LibraryManagement.Core.Commands.Borrowing;

public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
{
    public BorrowBookCommandValidator()
    {
        RuleFor(x => x.Borrowing.BookId).GreaterThan(0);
        RuleFor(x => x.Borrowing.MemberId).GreaterThan(0);
        RuleFor(x => x.Borrowing.BorrowedById).GreaterThan(0);
        RuleFor(x => x.Borrowing.BorrowDays).InclusiveBetween(1, 365);
        RuleFor(x => x.Borrowing.Notes).MaximumLength(1000);
    }
}

public class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnBookCommandValidator()
    {
        RuleFor(x => x.Return.TransactionId).GreaterThan(0);
        RuleFor(x => x.Return.ReturnedById).GreaterThan(0);
        RuleFor(x => x.Return.Notes).MaximumLength(1000);
    }
}

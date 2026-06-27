using FluentValidation;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Queries.Books;

namespace LibraryManagement.Core.Queries.Books;

public class SearchBooksQueryValidator : AbstractValidator<SearchBooksQuery> { }

public class GetBooksByStatusQueryValidator : AbstractValidator<GetBooksByStatusQuery>
{
    public GetBooksByStatusQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

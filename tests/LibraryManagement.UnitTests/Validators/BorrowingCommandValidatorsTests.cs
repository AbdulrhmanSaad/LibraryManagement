using FluentValidation.TestHelper;
using LibraryManagement.Core.Commands.Borrowing;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.UnitTests.Validators;

public class BorrowBookCommandValidatorTests
{
    private readonly BorrowBookCommandValidator _validator = new();

    [Fact]
    public async Task BorrowBookCommandValidator_ShouldHaveError_WhenBookIdIsZero()
    {
        var command = new BorrowBookCommand(new CreateBorrowingDto { BookId = 0, MemberId = 1, BorrowedById = 1, BorrowDays = 14 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Borrowing.BookId);
    }

    [Fact]
    public async Task BorrowBookCommandValidator_ShouldHaveError_WhenMemberIdIsZero()
    {
        var command = new BorrowBookCommand(new CreateBorrowingDto { BookId = 1, MemberId = 0, BorrowedById = 1, BorrowDays = 14 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Borrowing.MemberId);
    }

    [Fact]
    public async Task BorrowBookCommandValidator_ShouldHaveError_WhenBorrowedByIdIsZero()
    {
        var command = new BorrowBookCommand(new CreateBorrowingDto { BookId = 1, MemberId = 1, BorrowedById = 0, BorrowDays = 14 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Borrowing.BorrowedById);
    }

    [Fact]
    public async Task BorrowBookCommandValidator_ShouldHaveError_WhenBorrowDaysIsZero()
    {
        var command = new BorrowBookCommand(new CreateBorrowingDto { BookId = 1, MemberId = 1, BorrowedById = 1, BorrowDays = 0 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Borrowing.BorrowDays);
    }

    [Fact]
    public async Task BorrowBookCommandValidator_ShouldHaveError_WhenBorrowDaysExceedsMax()
    {
        var command = new BorrowBookCommand(new CreateBorrowingDto { BookId = 1, MemberId = 1, BorrowedById = 1, BorrowDays = 366 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Borrowing.BorrowDays);
    }

    [Fact]
    public async Task BorrowBookCommandValidator_ShouldHaveError_WhenNotesExceedsMaxLength()
    {
        var command = new BorrowBookCommand(new CreateBorrowingDto { BookId = 1, MemberId = 1, BorrowedById = 1, BorrowDays = 14, Notes = new string('X', 1001) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Borrowing.Notes);
    }

    [Fact]
    public async Task BorrowBookCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new BorrowBookCommand(new CreateBorrowingDto { BookId = 1, MemberId = 1, BorrowedById = 1, BorrowDays = 14 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task BorrowBookCommandValidator_ShouldNotHaveError_WhenBorrowDaysAtMinimum()
    {
        var command = new BorrowBookCommand(new CreateBorrowingDto { BookId = 1, MemberId = 1, BorrowedById = 1, BorrowDays = 1 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task BorrowBookCommandValidator_ShouldNotHaveError_WhenBorrowDaysAtMaximum()
    {
        var command = new BorrowBookCommand(new CreateBorrowingDto { BookId = 1, MemberId = 1, BorrowedById = 1, BorrowDays = 365 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class ReturnBookCommandValidatorTests
{
    private readonly ReturnBookCommandValidator _validator = new();

    [Fact]
    public async Task ReturnBookCommandValidator_ShouldHaveError_WhenTransactionIdIsZero()
    {
        var command = new ReturnBookCommand(new ReturnBookDto { TransactionId = 0, ReturnedById = 1 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Return.TransactionId);
    }

    [Fact]
    public async Task ReturnBookCommandValidator_ShouldHaveError_WhenReturnedByIdIsZero()
    {
        var command = new ReturnBookCommand(new ReturnBookDto { TransactionId = 1, ReturnedById = 0 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Return.ReturnedById);
    }

    [Fact]
    public async Task ReturnBookCommandValidator_ShouldHaveError_WhenNotesExceedsMaxLength()
    {
        var command = new ReturnBookCommand(new ReturnBookDto { TransactionId = 1, ReturnedById = 1, Notes = new string('X', 1001) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Return.Notes);
    }

    [Fact]
    public async Task ReturnBookCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new ReturnBookCommand(new ReturnBookDto { TransactionId = 1, ReturnedById = 1 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task ReturnBookCommandValidator_ShouldNotHaveError_WhenNotesIsNull()
    {
        var command = new ReturnBookCommand(new ReturnBookDto { TransactionId = 1, ReturnedById = 1, Notes = null });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

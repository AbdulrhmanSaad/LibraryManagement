using FluentValidation.TestHelper;
using LibraryManagement.Core.Commands.Books;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.UnitTests.Validators;

public class CreateBookCommandValidatorTests
{
    private readonly CreateBookCommandValidator _validator = new();

    [Fact]
    public async Task CreateBookCommandValidator_ShouldHaveError_WhenISBNIsEmpty()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = "", Title = "Test" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.ISBN);
    }

    [Fact]
    public async Task CreateBookCommandValidator_ShouldHaveError_WhenISBNExceedsMaxLength()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = new string('X', 21), Title = "Test" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.ISBN);
    }

    [Fact]
    public async Task CreateBookCommandValidator_ShouldHaveError_WhenTitleIsEmpty()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = "12345", Title = "" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.Title);
    }

    [Fact]
    public async Task CreateBookCommandValidator_ShouldHaveError_WhenTitleExceedsMaxLength()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = "12345", Title = new string('X', 501) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.Title);
    }

    [Fact]
    public async Task CreateBookCommandValidator_ShouldHaveError_WhenLanguageExceedsMaxLength()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = "12345", Title = "Test", Language = new string('X', 51) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.Language);
    }

    [Fact]
    public async Task CreateBookCommandValidator_ShouldHaveError_WhenPublicationYearOutOfRange()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = "12345", Title = "Test", PublicationYear = 999 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.PublicationYear);
    }

    [Fact]
    public async Task CreateBookCommandValidator_ShouldHaveError_WhenPublicationYearTooHigh()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = "12345", Title = "Test", PublicationYear = 2101 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.PublicationYear);
    }

    [Fact]
    public async Task CreateBookCommandValidator_ShouldHaveError_WhenPageCountNotPositive()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = "12345", Title = "Test", PageCount = -1 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.PageCount);
    }

    [Fact]
    public async Task CreateBookCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = "978-0-123456", Title = "Valid Book", Language = "English", PublicationYear = 2023, PageCount = 300 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateBookCommandValidator_ShouldNotHaveError_WhenNullableFieldsAreNull()
    {
        var command = new CreateBookCommand(new CreateBookDto { ISBN = "978-0-123456", Title = "Valid Book" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class UpdateBookCommandValidatorTests
{
    private readonly UpdateBookCommandValidator _validator = new();

    [Fact]
    public async Task UpdateBookCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new UpdateBookCommand(0, new UpdateBookDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdateBookCommandValidator_ShouldHaveError_WhenISBNExceedsMaxLength()
    {
        var command = new UpdateBookCommand(1, new UpdateBookDto { ISBN = new string('X', 21) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.ISBN);
    }

    [Fact]
    public async Task UpdateBookCommandValidator_ShouldHaveError_WhenTitleExceedsMaxLength()
    {
        var command = new UpdateBookCommand(1, new UpdateBookDto { Title = new string('X', 501) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.Title);
    }

    [Fact]
    public async Task UpdateBookCommandValidator_ShouldHaveError_WhenLanguageExceedsMaxLength()
    {
        var command = new UpdateBookCommand(1, new UpdateBookDto { Language = new string('X', 51) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.Language);
    }

    [Fact]
    public async Task UpdateBookCommandValidator_ShouldHaveError_WhenPublicationYearOutOfRange()
    {
        var command = new UpdateBookCommand(1, new UpdateBookDto { PublicationYear = 999 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.PublicationYear);
    }

    [Fact]
    public async Task UpdateBookCommandValidator_ShouldHaveError_WhenPageCountNotPositive()
    {
        var command = new UpdateBookCommand(1, new UpdateBookDto { PageCount = -1 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Book.PageCount);
    }

    [Fact]
    public async Task UpdateBookCommandValidator_ShouldNotHaveError_WhenAllFieldsNull()
    {
        var command = new UpdateBookCommand(1, new UpdateBookDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateBookCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new UpdateBookCommand(1, new UpdateBookDto { ISBN = "978-0-123456", Title = "Updated Title", Language = "French", PublicationYear = 2020, PageCount = 400 });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class DeleteBookCommandValidatorTests
{
    private readonly DeleteBookCommandValidator _validator = new();

    [Fact]
    public async Task DeleteBookCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new DeleteBookCommand(0);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task DeleteBookCommandValidator_ShouldNotHaveError_WhenIdIsPositive()
    {
        var command = new DeleteBookCommand(1);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

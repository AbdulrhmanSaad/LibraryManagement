using FluentValidation.TestHelper;
using LibraryManagement.Core.Commands.Authors;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.UnitTests.Validators;

public class CreateAuthorCommandValidatorTests
{
    private readonly CreateAuthorCommandValidator _validator = new();

    [Fact]
    public async Task CreateAuthorCommandValidator_ShouldHaveError_WhenFirstNameIsEmpty()
    {
        var command = new CreateAuthorCommand(new CreateAuthorDto { FirstName = "", LastName = "Doe" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Author.FirstName);
    }

    [Fact]
    public async Task CreateAuthorCommandValidator_ShouldHaveError_WhenFirstNameExceedsMaxLength()
    {
        var command = new CreateAuthorCommand(new CreateAuthorDto { FirstName = new string('X', 101), LastName = "Doe" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Author.FirstName);
    }

    [Fact]
    public async Task CreateAuthorCommandValidator_ShouldHaveError_WhenLastNameIsEmpty()
    {
        var command = new CreateAuthorCommand(new CreateAuthorDto { FirstName = "John", LastName = "" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Author.LastName);
    }

    [Fact]
    public async Task CreateAuthorCommandValidator_ShouldHaveError_WhenLastNameExceedsMaxLength()
    {
        var command = new CreateAuthorCommand(new CreateAuthorDto { FirstName = "John", LastName = new string('X', 101) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Author.LastName);
    }

    [Fact]
    public async Task CreateAuthorCommandValidator_ShouldHaveError_WhenBiographyExceedsMaxLength()
    {
        var command = new CreateAuthorCommand(new CreateAuthorDto { FirstName = "John", LastName = "Doe", Biography = new string('B', 4001) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Author.Biography);
    }

    [Fact]
    public async Task CreateAuthorCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new CreateAuthorCommand(new CreateAuthorDto { FirstName = "John", LastName = "Doe" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateAuthorCommandValidator_ShouldNotHaveError_WhenBiographyIsNull()
    {
        var command = new CreateAuthorCommand(new CreateAuthorDto { FirstName = "John", LastName = "Doe", Biography = null });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class UpdateAuthorCommandValidatorTests
{
    private readonly UpdateAuthorCommandValidator _validator = new();

    [Fact]
    public async Task UpdateAuthorCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new UpdateAuthorCommand(0, new UpdateAuthorDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdateAuthorCommandValidator_ShouldHaveError_WhenFirstNameExceedsMaxLength()
    {
        var command = new UpdateAuthorCommand(1, new UpdateAuthorDto { FirstName = new string('X', 101) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Author.FirstName);
    }

    [Fact]
    public async Task UpdateAuthorCommandValidator_ShouldHaveError_WhenLastNameExceedsMaxLength()
    {
        var command = new UpdateAuthorCommand(1, new UpdateAuthorDto { LastName = new string('X', 101) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Author.LastName);
    }

    [Fact]
    public async Task UpdateAuthorCommandValidator_ShouldHaveError_WhenBiographyExceedsMaxLength()
    {
        var command = new UpdateAuthorCommand(1, new UpdateAuthorDto { Biography = new string('B', 4001) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Author.Biography);
    }

    [Fact]
    public async Task UpdateAuthorCommandValidator_ShouldNotHaveError_WhenAllFieldsNull()
    {
        var command = new UpdateAuthorCommand(1, new UpdateAuthorDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateAuthorCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new UpdateAuthorCommand(1, new UpdateAuthorDto { FirstName = "Jane", LastName = "Smith", Biography = "Updated bio" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class DeleteAuthorCommandValidatorTests
{
    private readonly DeleteAuthorCommandValidator _validator = new();

    [Fact]
    public async Task DeleteAuthorCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new DeleteAuthorCommand(0);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task DeleteAuthorCommandValidator_ShouldNotHaveError_WhenIdIsPositive()
    {
        var command = new DeleteAuthorCommand(1);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

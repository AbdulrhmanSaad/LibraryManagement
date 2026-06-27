using FluentValidation.TestHelper;
using LibraryManagement.Core.Commands.Publishers;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.UnitTests.Validators;

public class CreatePublisherCommandValidatorTests
{
    private readonly CreatePublisherCommandValidator _validator = new();

    [Fact]
    public async Task CreatePublisherCommandValidator_ShouldHaveError_WhenNameIsEmpty()
    {
        var command = new CreatePublisherCommand(new CreatePublisherDto { Name = "" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Publisher.Name);
    }

    [Fact]
    public async Task CreatePublisherCommandValidator_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        var command = new CreatePublisherCommand(new CreatePublisherDto { Name = new string('X', 201) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Publisher.Name);
    }

    [Fact]
    public async Task CreatePublisherCommandValidator_ShouldHaveError_WhenEmailIsInvalid()
    {
        var command = new CreatePublisherCommand(new CreatePublisherDto { Name = "Test Pub", Email = "invalid" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Publisher.Email);
    }

    [Fact]
    public async Task CreatePublisherCommandValidator_ShouldHaveError_WhenEmailExceedsMaxLength()
    {
        var command = new CreatePublisherCommand(new CreatePublisherDto { Name = "Test Pub", Email = new string('x', 256) + "@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Publisher.Email);
    }

    [Fact]
    public async Task CreatePublisherCommandValidator_ShouldHaveError_WhenWebsiteExceedsMaxLength()
    {
        var command = new CreatePublisherCommand(new CreatePublisherDto { Name = "Test Pub", Website = new string('W', 256) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Publisher.Website);
    }

    [Fact]
    public async Task CreatePublisherCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new CreatePublisherCommand(new CreatePublisherDto { Name = "Penguin Books" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreatePublisherCommandValidator_ShouldNotHaveError_WhenOptionalFieldsAreNull()
    {
        var command = new CreatePublisherCommand(new CreatePublisherDto { Name = "Penguin Books", Email = null, Website = null });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class UpdatePublisherCommandValidatorTests
{
    private readonly UpdatePublisherCommandValidator _validator = new();

    [Fact]
    public async Task UpdatePublisherCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new UpdatePublisherCommand(0, new UpdatePublisherDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdatePublisherCommandValidator_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        var command = new UpdatePublisherCommand(1, new UpdatePublisherDto { Name = new string('X', 201) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Publisher.Name);
    }

    [Fact]
    public async Task UpdatePublisherCommandValidator_ShouldNotHaveError_WhenAllFieldsNull()
    {
        var command = new UpdatePublisherCommand(1, new UpdatePublisherDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdatePublisherCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new UpdatePublisherCommand(1, new UpdatePublisherDto { Name = "Updated Pub" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class DeletePublisherCommandValidatorTests
{
    private readonly DeletePublisherCommandValidator _validator = new();

    [Fact]
    public async Task DeletePublisherCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new DeletePublisherCommand(0);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task DeletePublisherCommandValidator_ShouldNotHaveError_WhenIdIsPositive()
    {
        var command = new DeletePublisherCommand(1);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

using FluentValidation.TestHelper;
using LibraryManagement.Core.Commands.Categories;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.UnitTests.Validators;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public async Task CreateCategoryCommandValidator_ShouldHaveError_WhenNameIsEmpty()
    {
        var command = new CreateCategoryCommand(new CreateCategoryDto { Name = "" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Category.Name);
    }

    [Fact]
    public async Task CreateCategoryCommandValidator_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        var command = new CreateCategoryCommand(new CreateCategoryDto { Name = new string('X', 201) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Category.Name);
    }

    [Fact]
    public async Task CreateCategoryCommandValidator_ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        var command = new CreateCategoryCommand(new CreateCategoryDto { Name = "Test", Description = new string('D', 501) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Category.Description);
    }

    [Fact]
    public async Task CreateCategoryCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new CreateCategoryCommand(new CreateCategoryDto { Name = "Fiction", Description = "Fiction books" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateCategoryCommandValidator_ShouldNotHaveError_WhenDescriptionIsNull()
    {
        var command = new CreateCategoryCommand(new CreateCategoryDto { Name = "Fiction", Description = null });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    [Fact]
    public async Task UpdateCategoryCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new UpdateCategoryCommand(0, new UpdateCategoryDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdateCategoryCommandValidator_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        var command = new UpdateCategoryCommand(1, new UpdateCategoryDto { Name = new string('X', 201) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Category.Name);
    }

    [Fact]
    public async Task UpdateCategoryCommandValidator_ShouldNotHaveError_WhenAllFieldsNull()
    {
        var command = new UpdateCategoryCommand(1, new UpdateCategoryDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateCategoryCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new UpdateCategoryCommand(1, new UpdateCategoryDto { Name = "Updated Name" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class DeleteCategoryCommandValidatorTests
{
    private readonly DeleteCategoryCommandValidator _validator = new();

    [Fact]
    public async Task DeleteCategoryCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new DeleteCategoryCommand(0);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task DeleteCategoryCommandValidator_ShouldNotHaveError_WhenIdIsPositive()
    {
        var command = new DeleteCategoryCommand(1);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

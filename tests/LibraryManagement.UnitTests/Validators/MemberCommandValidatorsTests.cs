using FluentValidation.TestHelper;
using LibraryManagement.Core.Commands.Members;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.UnitTests.Validators;

public class CreateMemberCommandValidatorTests
{
    private readonly CreateMemberCommandValidator _validator = new();

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldHaveError_WhenFirstNameIsEmpty()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "", LastName = "Doe", Email = "test@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.FirstName);
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldHaveError_WhenFirstNameExceedsMaxLength()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = new string('X', 101), LastName = "Doe", Email = "test@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.FirstName);
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldHaveError_WhenLastNameIsEmpty()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "John", LastName = "", Email = "test@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.LastName);
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldHaveError_WhenLastNameExceedsMaxLength()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "John", LastName = new string('X', 101), Email = "test@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.LastName);
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldHaveError_WhenEmailIsEmpty()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "John", LastName = "Doe", Email = "" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.Email);
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldHaveError_WhenEmailIsInvalid()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "John", LastName = "Doe", Email = "invalid" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.Email);
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldHaveError_WhenEmailExceedsMaxLength()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "John", LastName = "Doe", Email = new string('x', 256) + "@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.Email);
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldHaveError_WhenPhoneExceedsMaxLength()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "John", LastName = "Doe", Email = "test@test.com", Phone = new string('5', 51) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.Phone);
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldHaveError_WhenAddressExceedsMaxLength()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "John", LastName = "Doe", Email = "test@test.com", Address = new string('A', 501) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.Address);
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateMemberCommandValidator_ShouldNotHaveError_WhenOptionalFieldsAreNull()
    {
        var command = new CreateMemberCommand(new CreateMemberDto { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com", Phone = null, Address = null });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class UpdateMemberCommandValidatorTests
{
    private readonly UpdateMemberCommandValidator _validator = new();

    [Fact]
    public async Task UpdateMemberCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new UpdateMemberCommand(0, new UpdateMemberDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdateMemberCommandValidator_ShouldHaveError_WhenFirstNameExceedsMaxLength()
    {
        var command = new UpdateMemberCommand(1, new UpdateMemberDto { FirstName = new string('X', 101) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.FirstName);
    }

    [Fact]
    public async Task UpdateMemberCommandValidator_ShouldHaveError_WhenLastNameExceedsMaxLength()
    {
        var command = new UpdateMemberCommand(1, new UpdateMemberDto { LastName = new string('X', 101) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.LastName);
    }

    [Fact]
    public async Task UpdateMemberCommandValidator_ShouldHaveError_WhenEmailIsInvalid()
    {
        var command = new UpdateMemberCommand(1, new UpdateMemberDto { Email = "invalid" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.Email);
    }

    [Fact]
    public async Task UpdateMemberCommandValidator_ShouldHaveError_WhenEmailExceedsMaxLength()
    {
        var command = new UpdateMemberCommand(1, new UpdateMemberDto { Email = new string('x', 256) + "@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Member.Email);
    }

    [Fact]
    public async Task UpdateMemberCommandValidator_ShouldNotHaveError_WhenAllFieldsNull()
    {
        var command = new UpdateMemberCommand(1, new UpdateMemberDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateMemberCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new UpdateMemberCommand(1, new UpdateMemberDto { FirstName = "John", LastName = "Smith", Email = "john.smith@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class DeleteMemberCommandValidatorTests
{
    private readonly DeleteMemberCommandValidator _validator = new();

    [Fact]
    public async Task DeleteMemberCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new DeleteMemberCommand(0);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task DeleteMemberCommandValidator_ShouldNotHaveError_WhenIdIsPositive()
    {
        var command = new DeleteMemberCommand(1);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

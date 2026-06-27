using FluentValidation.TestHelper;
using LibraryManagement.Core.Commands.Users;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.UnitTests.Validators;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenUsernameIsEmpty()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "", Email = "test@test.com", Password = "pass123", FullName = "Test User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Username);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenUsernameIsTooShort()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "ab", Email = "test@test.com", Password = "pass123", FullName = "Test User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Username);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenUsernameExceedsMaxLength()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = new string('x', 101), Email = "test@test.com", Password = "pass123", FullName = "Test User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Username);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenEmailIsEmpty()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = "", Password = "pass123", FullName = "Test User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Email);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenEmailIsInvalid()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = "invalid", Password = "pass123", FullName = "Test User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Email);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenEmailExceedsMaxLength()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = new string('x', 256) + "@test.com", Password = "pass123", FullName = "Test User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Email);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenPasswordIsEmpty()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = "test@test.com", Password = "", FullName = "Test User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Password);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenPasswordIsTooShort()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = "test@test.com", Password = "12345", FullName = "Test User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Password);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenPasswordExceedsMaxLength()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = "test@test.com", Password = new string('x', 101), FullName = "Test User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Password);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenFullNameIsEmpty()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = "test@test.com", Password = "pass123", FullName = "", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.FullName);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenFullNameExceedsMaxLength()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = "test@test.com", Password = "pass123", FullName = new string('X', 201), Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.FullName);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenRoleIsEmpty()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = "test@test.com", Password = "pass123", FullName = "Test User", Role = "" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Role);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldHaveError_WhenRoleIsInvalid()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "testuser", Email = "test@test.com", Password = "pass123", FullName = "Test User", Role = "Manager" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Role);
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldNotHaveError_WhenValidAsAdministrator()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "admin", Email = "admin@test.com", Password = "pass123", FullName = "Admin User", Role = "Administrator" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldNotHaveError_WhenValidAsLibrarian()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "librarian", Email = "lib@test.com", Password = "pass123", FullName = "Lib User", Role = "Librarian" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldNotHaveError_WhenValidAsStaff()
    {
        var command = new CreateUserCommand(new CreateUserDto { Username = "staff", Email = "staff@test.com", Password = "pass123", FullName = "Staff User", Role = "Staff" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class UpdateUserCommandValidatorTests
{
    private readonly UpdateUserCommandValidator _validator = new();

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new UpdateUserCommand(0, new UpdateUserDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldHaveError_WhenEmailIsInvalid()
    {
        var command = new UpdateUserCommand(1, new UpdateUserDto { Email = "invalid" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Email);
    }

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldHaveError_WhenEmailExceedsMaxLength()
    {
        var command = new UpdateUserCommand(1, new UpdateUserDto { Email = new string('x', 256) + "@test.com" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Email);
    }

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldHaveError_WhenFullNameExceedsMaxLength()
    {
        var command = new UpdateUserCommand(1, new UpdateUserDto { FullName = new string('X', 201) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.FullName);
    }

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldHaveError_WhenPasswordIsTooShort()
    {
        var command = new UpdateUserCommand(1, new UpdateUserDto { Password = "12345" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Password);
    }

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldHaveError_WhenPasswordExceedsMaxLength()
    {
        var command = new UpdateUserCommand(1, new UpdateUserDto { Password = new string('x', 101) });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.User.Password);
    }

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldNotHaveError_WhenAllFieldsNull()
    {
        var command = new UpdateUserCommand(1, new UpdateUserDto());
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new UpdateUserCommand(1, new UpdateUserDto { Email = "updated@test.com", FullName = "Updated User" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class DeleteUserCommandValidatorTests
{
    private readonly DeleteUserCommandValidator _validator = new();

    [Fact]
    public async Task DeleteUserCommandValidator_ShouldHaveError_WhenIdIsZero()
    {
        var command = new DeleteUserCommand(0);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task DeleteUserCommandValidator_ShouldNotHaveError_WhenIdIsPositive()
    {
        var command = new DeleteUserCommand(1);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

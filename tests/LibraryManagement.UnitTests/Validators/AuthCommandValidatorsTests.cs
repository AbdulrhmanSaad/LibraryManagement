using FluentValidation.TestHelper;
using LibraryManagement.Core.Commands.Auth;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.UnitTests.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public async Task LoginCommandValidator_ShouldHaveError_WhenUsernameIsEmpty()
    {
        var command = new LoginCommand(new LoginDto { Username = "", Password = "pass123" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Login.Username);
    }

    [Fact]
    public async Task LoginCommandValidator_ShouldHaveError_WhenPasswordIsEmpty()
    {
        var command = new LoginCommand(new LoginDto { Username = "admin", Password = "" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Login.Password);
    }

    [Fact]
    public async Task LoginCommandValidator_ShouldNotHaveError_WhenValid()
    {
        var command = new LoginCommand(new LoginDto { Username = "admin", Password = "pass123" });
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public async Task RefreshTokenCommandValidator_ShouldHaveError_WhenTokenIsEmpty()
    {
        var command = new RefreshTokenCommand("");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenCommandValidator_ShouldNotHaveError_WhenTokenIsProvided()
    {
        var command = new RefreshTokenCommand("some-valid-token");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class RevokeTokenCommandValidatorTests
{
    private readonly RevokeTokenCommandValidator _validator = new();

    [Fact]
    public async Task RevokeTokenCommandValidator_ShouldHaveError_WhenTokenIsEmpty()
    {
        var command = new RevokeTokenCommand("");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public async Task RevokeTokenCommandValidator_ShouldNotHaveError_WhenTokenIsProvided()
    {
        var command = new RevokeTokenCommand("some-valid-token");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

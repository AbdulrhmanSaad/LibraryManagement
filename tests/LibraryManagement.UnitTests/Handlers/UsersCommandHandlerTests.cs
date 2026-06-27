using AutoMapper;
using Moq;
using FluentAssertions;
using LibraryManagement.Core.Commands.Users;
using LibraryManagement.Core.Commands.Users.Handlers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.UnitTests.Helpers;

namespace LibraryManagement.UnitTests.Handlers;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<MockUserManager> _userManagerMock;
    private readonly MockRoleManager _roleManager;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userManagerMock = new Mock<MockUserManager> { CallBase = true };
        _roleManager = new MockRoleManager();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateUserCommandHandler(_userManagerMock.Object, _roleManager, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DuplicateUsername_ThrowsInvalidOperationException()
    {
        var dto = new CreateUserDto { Username = "existing", Email = "test@test.com", Password = "P@ssw0rd", Role = "Staff" };
        _userManagerMock.Setup(x => x.FindByNameAsync("existing")).ReturnsAsync(new AppUser { UserName = "existing" });

        var act = () => _handler.Handle(new CreateUserCommand(dto), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Username already exists");
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var dto = new CreateUserDto { Username = "newuser", Email = "dup@test.com", Password = "P@ssw0rd", Role = "Staff" };
        _userManagerMock.Setup(x => x.FindByNameAsync("newuser")).ReturnsAsync((AppUser?)null);
        _userManagerMock.Setup(x => x.FindByEmailAsync("dup@test.com")).ReturnsAsync(new AppUser { Email = "dup@test.com" });

        var act = () => _handler.Handle(new CreateUserCommand(dto), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Email already exists");
    }

    [Fact]
    public async Task Handle_HappyPath_CreatesUserAndAddsToRole()
    {
        var dto = new CreateUserDto { Username = "newuser", Email = "new@test.com", Password = "P@ssw0rd", FullName = "New User", Role = "Staff" };
        var user = new AppUser { Id = 1, UserName = "newuser", Email = "new@test.com", FullName = "New User" };
        var userDto = new UserDto { Id = 1, Username = "newuser", Email = "new@test.com", FullName = "New User", Role = "Staff" };

        _userManagerMock.Setup(x => x.FindByNameAsync("newuser")).ReturnsAsync((AppUser?)null);
        _userManagerMock.Setup(x => x.FindByEmailAsync("new@test.com")).ReturnsAsync((AppUser?)null);
        _mapperMock.Setup(m => m.Map<AppUser>(dto)).Returns(user);
        _userManagerMock.Setup(x => x.CreateAsync(user, "P@ssw0rd")).ReturnsAsync(Microsoft.AspNetCore.Identity.IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(user, "Staff")).ReturnsAsync(Microsoft.AspNetCore.Identity.IdentityResult.Success);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        var result = await _handler.Handle(new CreateUserCommand(dto), default);

        result.Should().Be(userDto);
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _userManagerMock.Verify(x => x.CreateAsync(user, "P@ssw0rd"), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(user, "Staff"), Times.Once);
    }
}

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<MockUserManager> _userManagerMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userManagerMock = new Mock<MockUserManager> { CallBase = true };
        _handler = new UpdateUserCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        _userManagerMock.Setup(x => x.FindByIdAsync("999")).ReturnsAsync((AppUser?)null);

        var act = () => _handler.Handle(new UpdateUserCommand(999, new UpdateUserDto { Email = "new@test.com" }), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("User not found");
    }

    [Fact]
    public async Task Handle_HappyPath_UpdatesUser()
    {
        var user = new AppUser { Id = 1, UserName = "test", Email = "old@test.com", FullName = "Old Name" };
        _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);

        await _handler.Handle(new UpdateUserCommand(1, new UpdateUserDto
        {
            Email = "new@test.com",
            FullName = "New Name",
            IsActive = false
        }), default);

        user.Email.Should().Be("new@test.com");
        user.FullName.Should().Be("New Name");
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_WithPasswordUpdate_ResetsPassword()
    {
        var user = new AppUser { Id = 1, UserName = "test", Email = "test@test.com" };
        _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);

        await _handler.Handle(new UpdateUserCommand(1, new UpdateUserDto { Password = "NewP@ssw0rd" }), default);

        _userManagerMock.Verify(x => x.GeneratePasswordResetTokenAsync(user), Times.Once);
        _userManagerMock.Verify(x => x.ResetPasswordAsync(user, "reset-token", "NewP@ssw0rd"), Times.Once);
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }
}

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<MockUserManager> _userManagerMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _userManagerMock = new Mock<MockUserManager> { CallBase = true };
        _handler = new DeleteUserCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        _userManagerMock.Setup(x => x.FindByIdAsync("999")).ReturnsAsync((AppUser?)null);

        var act = () => _handler.Handle(new DeleteUserCommand(999), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("User not found");
    }

    [Fact]
    public async Task Handle_HappyPath_DeactivatesUser()
    {
        var user = new AppUser { Id = 1, UserName = "test", IsActive = true };
        _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);

        await _handler.Handle(new DeleteUserCommand(1), default);

        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }
}

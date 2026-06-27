using System.Linq.Expressions;
using AutoMapper;
using Moq;
using FluentAssertions;
using LibraryManagement.Core.Commands.Members;
using LibraryManagement.Core.Commands.Members.Handlers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.UnitTests.Handlers;

public class CreateMemberCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Member>> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateMemberCommandHandler _handler;

    public CreateMemberCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Member>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateMemberCommandHandler(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var dto = new CreateMemberDto { Email = "existing@test.com" };
        _repoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Member, bool>>>()))
            .ReturnsAsync(new List<Member> { new() });

        var act = () => _handler.Handle(new CreateMemberCommand(dto), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("A member with this email already exists");
    }

    [Fact]
    public async Task Handle_HappyPath_CreatesMember()
    {
        var dto = new CreateMemberDto { FirstName = "John", LastName = "Doe", Email = "john@test.com" };
        var entity = new Member { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" };
        var resultDto = new MemberDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com" };

        _repoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Member, bool>>>()))
            .ReturnsAsync(new List<Member>());
        _mapperMock.Setup(m => m.Map<Member>(dto)).Returns(entity);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Member>())).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<MemberDto>(entity)).Returns(resultDto);

        var result = await _handler.Handle(new CreateMemberCommand(dto), default);

        result.Should().Be(resultDto);
        entity.MemberNumber.Should().StartWith("MEM-");
        entity.MembershipDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
    }
}

public class UpdateMemberCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Member>> _repoMock;
    private readonly UpdateMemberCommandHandler _handler;

    public UpdateMemberCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Member>>();
        _handler = new UpdateMemberCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_MemberNotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Member?)null);

        var act = () => _handler.Handle(new UpdateMemberCommand(999, new UpdateMemberDto { FirstName = "Updated" }), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Member not found");
    }

    [Fact]
    public async Task Handle_HappyPath_UpdatesMember()
    {
        var existing = new Member { Id = 1, FirstName = "Old", LastName = "Name", Email = "old@test.com", Phone = "123", Address = "Addr", DateOfBirth = new DateTime(1990, 1, 1), IsActive = true };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await _handler.Handle(new UpdateMemberCommand(1, new UpdateMemberDto
        {
            FirstName = "New",
            LastName = "Name",
            Email = "new@test.com",
            Phone = "456",
            Address = "New addr",
            DateOfBirth = new DateTime(2000, 1, 1),
            IsActive = false
        }), default);

        existing.FirstName.Should().Be("New");
        existing.Email.Should().Be("new@test.com");
        existing.Phone.Should().Be("456");
        existing.Address.Should().Be("New addr");
        existing.DateOfBirth.Should().Be(new DateTime(2000, 1, 1));
        existing.IsActive.Should().BeFalse();
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyUpdatesNonNullFields()
    {
        var existing = new Member { Id = 1, FirstName = "Old", LastName = "Name", Email = "old@test.com", IsActive = true };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await _handler.Handle(new UpdateMemberCommand(1, new UpdateMemberDto { FirstName = "New" }), default);

        existing.FirstName.Should().Be("New");
        existing.LastName.Should().Be("Name");
        existing.Email.Should().Be("old@test.com");
        existing.IsActive.Should().BeTrue();
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }
}

public class DeleteMemberCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Member>> _repoMock;
    private readonly DeleteMemberCommandHandler _handler;

    public DeleteMemberCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Member>>();
        _handler = new DeleteMemberCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_MemberNotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Member?)null);

        var act = () => _handler.Handle(new DeleteMemberCommand(999), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Member not found");
    }

    [Fact]
    public async Task Handle_HappyPath_SoftDeletesMember()
    {
        var entity = new Member { Id = 1, IsActive = true };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        await _handler.Handle(new DeleteMemberCommand(1), default);

        entity.IsActive.Should().BeFalse();
        _repoMock.Verify(r => r.UpdateAsync(entity), Times.Once);
    }
}

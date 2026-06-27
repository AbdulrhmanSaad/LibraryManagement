using AutoMapper;
using Moq;
using FluentAssertions;
using LibraryManagement.Core.Commands.Authors;
using LibraryManagement.Core.Commands.Authors.Handlers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.UnitTests.Handlers;

public class CreateAuthorCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Author>> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateAuthorCommandHandler _handler;

    public CreateAuthorCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Author>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateAuthorCommandHandler(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_HappyPath_ReturnsMappedDto()
    {
        var dto = new CreateAuthorDto { FirstName = "J", LastName = "K", Biography = "Bio" };
        var entity = new Author { Id = 1, FirstName = "J", LastName = "K" };
        var resultDto = new AuthorDto { Id = 1, FirstName = "J", LastName = "K" };

        _mapperMock.Setup(m => m.Map<Author>(dto)).Returns(entity);
        _repoMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<AuthorDto>(entity)).Returns(resultDto);

        var result = await _handler.Handle(new CreateAuthorCommand(dto), default);

        result.Should().Be(resultDto);
        _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
    }
}

public class UpdateAuthorCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Author>> _repoMock;
    private readonly UpdateAuthorCommandHandler _handler;

    public UpdateAuthorCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Author>>();
        _handler = new UpdateAuthorCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_AuthorNotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Author?)null);

        var act = () => _handler.Handle(new UpdateAuthorCommand(999, new UpdateAuthorDto { FirstName = "Updated" }), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Author not found");
    }

    [Fact]
    public async Task Handle_HappyPath_UpdatesAuthor()
    {
        var existing = new Author { Id = 1, FirstName = "Old", LastName = "Name" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await _handler.Handle(new UpdateAuthorCommand(1, new UpdateAuthorDto
        {
            FirstName = "New",
            LastName = "Name",
            Biography = "Updated bio",
            BirthDate = new DateTime(2000, 1, 1)
        }), default);

        existing.FirstName.Should().Be("New");
        existing.LastName.Should().Be("Name");
        existing.Biography.Should().Be("Updated bio");
        existing.BirthDate.Should().Be(new DateTime(2000, 1, 1));
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyUpdatesNonNullFields()
    {
        var existing = new Author { Id = 1, FirstName = "Old", LastName = "Name", Biography = "Bio", BirthDate = new DateTime(1990, 1, 1) };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await _handler.Handle(new UpdateAuthorCommand(1, new UpdateAuthorDto { FirstName = "New" }), default);

        existing.FirstName.Should().Be("New");
        existing.LastName.Should().Be("Name");
        existing.Biography.Should().Be("Bio");
        existing.BirthDate.Should().Be(new DateTime(1990, 1, 1));
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }
}

public class DeleteAuthorCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Author>> _repoMock;
    private readonly DeleteAuthorCommandHandler _handler;

    public DeleteAuthorCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Author>>();
        _handler = new DeleteAuthorCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_AuthorNotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Author?)null);

        var act = () => _handler.Handle(new DeleteAuthorCommand(999), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Author not found");
    }

    [Fact]
    public async Task Handle_HappyPath_DeletesAuthor()
    {
        var entity = new Author { Id = 1 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        await _handler.Handle(new DeleteAuthorCommand(1), default);

        _repoMock.Verify(r => r.DeleteAsync(entity), Times.Once);
    }
}

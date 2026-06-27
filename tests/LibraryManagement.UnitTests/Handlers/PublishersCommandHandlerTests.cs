using AutoMapper;
using Moq;
using FluentAssertions;
using LibraryManagement.Core.Commands.Publishers;
using LibraryManagement.Core.Commands.Publishers.Handlers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.UnitTests.Handlers;

public class CreatePublisherCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Publisher>> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreatePublisherCommandHandler _handler;

    public CreatePublisherCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Publisher>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreatePublisherCommandHandler(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_HappyPath_ReturnsMappedDto()
    {
        var dto = new CreatePublisherDto { Name = "Test Press", Email = "info@test.com" };
        var entity = new Publisher { Id = 1, Name = "Test Press" };
        var resultDto = new PublisherDto { Id = 1, Name = "Test Press" };

        _mapperMock.Setup(m => m.Map<Publisher>(dto)).Returns(entity);
        _repoMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<PublisherDto>(entity)).Returns(resultDto);

        var result = await _handler.Handle(new CreatePublisherCommand(dto), default);

        result.Should().Be(resultDto);
        _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
    }
}

public class UpdatePublisherCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Publisher>> _repoMock;
    private readonly UpdatePublisherCommandHandler _handler;

    public UpdatePublisherCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Publisher>>();
        _handler = new UpdatePublisherCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_PublisherNotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Publisher?)null);

        var act = () => _handler.Handle(new UpdatePublisherCommand(999, new UpdatePublisherDto { Name = "Updated" }), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Publisher not found");
    }

    [Fact]
    public async Task Handle_HappyPath_UpdatesPublisher()
    {
        var existing = new Publisher { Id = 1, Name = "Old", Address = "Addr", Phone = "123", Email = "old@test.com", Website = "old.com" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await _handler.Handle(new UpdatePublisherCommand(1, new UpdatePublisherDto
        {
            Name = "New",
            Address = "New addr",
            Phone = "456",
            Email = "new@test.com",
            Website = "new.com"
        }), default);

        existing.Name.Should().Be("New");
        existing.Address.Should().Be("New addr");
        existing.Phone.Should().Be("456");
        existing.Email.Should().Be("new@test.com");
        existing.Website.Should().Be("new.com");
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyUpdatesNonNullFields()
    {
        var existing = new Publisher { Id = 1, Name = "Old", Address = "Addr", Phone = "123", Email = "old@test.com", Website = "old.com" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await _handler.Handle(new UpdatePublisherCommand(1, new UpdatePublisherDto { Name = "New" }), default);

        existing.Name.Should().Be("New");
        existing.Address.Should().Be("Addr");
        existing.Phone.Should().Be("123");
        existing.Email.Should().Be("old@test.com");
        existing.Website.Should().Be("old.com");
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }
}

public class DeletePublisherCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Publisher>> _repoMock;
    private readonly DeletePublisherCommandHandler _handler;

    public DeletePublisherCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Publisher>>();
        _handler = new DeletePublisherCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_PublisherNotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Publisher?)null);

        var act = () => _handler.Handle(new DeletePublisherCommand(999), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Publisher not found");
    }

    [Fact]
    public async Task Handle_HappyPath_DeletesPublisher()
    {
        var entity = new Publisher { Id = 1 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        await _handler.Handle(new DeletePublisherCommand(1), default);

        _repoMock.Verify(r => r.DeleteAsync(entity), Times.Once);
    }
}

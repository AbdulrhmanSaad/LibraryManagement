using AutoMapper;
using Moq;
using FluentAssertions;
using LibraryManagement.Core.Commands.Categories;
using LibraryManagement.Core.Commands.Categories.Handlers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.UnitTests.Handlers;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Category>> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Category>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateCategoryCommandHandler(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_HappyPath_ReturnsMappedDto()
    {
        var dto = new CreateCategoryDto { Name = "Sci-Fi", Description = "Science fiction" };
        var entity = new Category { Id = 1, Name = "Sci-Fi" };
        var resultDto = new CategoryDto { Id = 1, Name = "Sci-Fi" };

        _mapperMock.Setup(m => m.Map<Category>(dto)).Returns(entity);
        _repoMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<CategoryDto>(entity)).Returns(resultDto);

        var result = await _handler.Handle(new CreateCategoryCommand(dto), default);

        result.Should().Be(resultDto);
        _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
    }
}

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Category>> _repoMock;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Category>>();
        _handler = new UpdateCategoryCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Category?)null);

        var act = () => _handler.Handle(new UpdateCategoryCommand(999, new UpdateCategoryDto { Name = "Updated" }), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Category not found");
    }

    [Fact]
    public async Task Handle_HappyPath_UpdatesCategory()
    {
        var existing = new Category { Id = 1, Name = "Old", Description = "Old desc", ParentCategoryId = null };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await _handler.Handle(new UpdateCategoryCommand(1, new UpdateCategoryDto
        {
            Name = "New",
            Description = "New desc",
            ParentCategoryId = 2
        }), default);

        existing.Name.Should().Be("New");
        existing.Description.Should().Be("New desc");
        existing.ParentCategoryId.Should().Be(2);
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyUpdatesNonNullOrChangedFields()
    {
        var existing = new Category { Id = 1, Name = "Old", Description = "Desc", ParentCategoryId = null };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        await _handler.Handle(new UpdateCategoryCommand(1, new UpdateCategoryDto { Name = "New" }), default);

        existing.Name.Should().Be("New");
        existing.Description.Should().Be("Desc");
        existing.ParentCategoryId.Should().BeNull();
        _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }
}

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<IGenericRepository<Category>> _repoMock;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _repoMock = new Mock<IGenericRepository<Category>>();
        _handler = new DeleteCategoryCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Category?)null);

        var act = () => _handler.Handle(new DeleteCategoryCommand(999), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Category not found");
    }

    [Fact]
    public async Task Handle_HappyPath_DeletesCategory()
    {
        var entity = new Category { Id = 1 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        await _handler.Handle(new DeleteCategoryCommand(1), default);

        _repoMock.Verify(r => r.DeleteAsync(entity), Times.Once);
    }
}

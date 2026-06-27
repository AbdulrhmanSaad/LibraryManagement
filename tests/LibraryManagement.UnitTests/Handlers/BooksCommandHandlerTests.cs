using AutoMapper;
using Azure.Core;
using FluentAssertions;
using LibraryManagement.Core.Commands.Books;
using LibraryManagement.Core.Commands.Books.Handlers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace LibraryManagement.UnitTests.Handlers;

public class CreateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IGenericRepository<Author>> _authorRepoMock;
    private readonly Mock<IGenericRepository<Category>> _categoryRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateBookCommandHandler _handler;

    public CreateBookCommandHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _authorRepoMock = new Mock<IGenericRepository<Author>>();
        _categoryRepoMock = new Mock<IGenericRepository<Category>>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateBookCommandHandler(
            _bookRepoMock.Object, _authorRepoMock.Object, _categoryRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DuplicateISBN_ThrowsInvalidOperationException()
    {
        var dto = new CreateBookDto { ISBN = "978-0-123456", Title = "Test" };
        _bookRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(new List<Book> { new() });

        var act = () => _handler.Handle(new CreateBookCommand(dto), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("A book with this ISBN already exists");
    }

    [Fact]
    public async Task Handle_HappyPath_CreatesBook()
    {
        var dto = new CreateBookDto
        {
            ISBN = "978-0-123456",
            Title = "New Book",
            Language = "English",
            AuthorIds = new List<int> { 1 },
            CategoryIds = new List<int> { 1 }
        };
        var createdBook = new Book { Id = 1 };
        var bookDto = new BookDto { Id = 1, Title = "New Book" };

        _bookRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Book, bool>>>()))
            .ReturnsAsync(new List<Book>());
        _authorRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Author, bool>>>()))
            .ReturnsAsync(new List<Author> { new() { Id = 1 } });
        _categoryRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(new List<Category> { new() { Id = 1 } });
        _bookRepoMock.Setup(r => r.AddAsync(It.IsAny<Book>())).ReturnsAsync(createdBook);
        _bookRepoMock.Setup(r => r.GetBookWithDetailsAsync(1)).ReturnsAsync(createdBook);
        _mapperMock.Setup(m => m.Map<Book>(dto)).Returns(createdBook);
        _mapperMock.Setup(m => m.Map<BookDto>(createdBook)).Returns(bookDto);

        var result = await _handler.Handle(new CreateBookCommand(dto), default);

        result.Should().Be(bookDto);
        createdBook.Status.Should().Be(BookStatus.In);
        _bookRepoMock.Verify(r => r.AddAsync(createdBook), Times.Once);
    }
}

public class UpdateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IGenericRepository<Author>> _authorRepoMock;
    private readonly Mock<IGenericRepository<Category>> _categoryRepoMock;
    private readonly UpdateBookCommandHandler _handler;

    public UpdateBookCommandHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _authorRepoMock = new Mock<IGenericRepository<Author>>();
        _categoryRepoMock = new Mock<IGenericRepository<Category>>();
        _handler = new UpdateBookCommandHandler(
            _bookRepoMock.Object, _authorRepoMock.Object, _categoryRepoMock.Object);
    }

    [Fact]
    public async Task Handle_BookNotFound_ThrowsKeyNotFoundException()
    {
        _bookRepoMock.Setup(r => r.GetBookWithDetailsAsync(999)).ReturnsAsync((Book?)null);

        var act = () => _handler.Handle(new UpdateBookCommand(999, new UpdateBookDto { Title = "Updated" }), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Book with Id '{999}' was not found.");
    }

    [Fact]
    public async Task Handle_HappyPath_UpdatesBook()
    {
        var book = new Book { Id = 1, Title = "Original", Authors = new List<Author>(), Categories = new List<Category>() };
        _bookRepoMock.Setup(r => r.GetBookWithDetailsAsync(1)).ReturnsAsync(book);

        var dto = new UpdateBookDto
        {
            Title = "Updated Title",
            AuthorIds = new List<int> { 2 },
            CategoryIds = new List<int> { 2 }
        };
        _authorRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Author, bool>>>()))
            .ReturnsAsync(new List<Author> { new() { Id = 2, FirstName = "J", LastName = "K" } });
        _categoryRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(new List<Category> { new() { Id = 2, Name = "Sci-Fi" } });

        await _handler.Handle(new UpdateBookCommand(1, dto), default);

        book.Title.Should().Be("Updated Title");
        book.Authors.Should().ContainSingle(a => a.Id == 2);
        book.Categories.Should().ContainSingle(c => c.Id == 2);
        _bookRepoMock.Verify(r => r.UpdateAsync(book), Times.Once);
    }
}

public class DeleteBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly DeleteBookCommandHandler _handler;

    public DeleteBookCommandHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _handler = new DeleteBookCommandHandler(_bookRepoMock.Object);
    }

    [Fact]
    public async Task Handle_BookNotFound_ThrowsKeyNotFoundException()
    {
        _bookRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Book?)null);

        var act = () => _handler.Handle(new DeleteBookCommand(999), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Book not found");
    }

    [Fact]
    public async Task Handle_HappyPath_ResetsBookToIn()
    {
        var book = new Book { Id = 1, Status = BookStatus.Out };
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);

        await _handler.Handle(new DeleteBookCommand(1), default);

        book.Status.Should().Be(BookStatus.In);
        _bookRepoMock.Verify(r => r.UpdateAsync(book), Times.Once);
    }
}

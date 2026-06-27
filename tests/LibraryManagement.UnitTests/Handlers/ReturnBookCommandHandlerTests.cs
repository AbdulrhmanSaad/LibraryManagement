using AutoMapper;
using Moq;
using FluentAssertions;
using LibraryManagement.Core.Commands.Borrowing;
using LibraryManagement.Core.Commands.Borrowing.Handlers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.UnitTests.Handlers;

public class ReturnBookCommandHandlerTests
{
    private readonly Mock<IBorrowingRepository> _borrowingRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ReturnBookCommandHandler _handler;

    public ReturnBookCommandHandlerTests()
    {
        _borrowingRepoMock = new Mock<IBorrowingRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new ReturnBookCommandHandler(
            _borrowingRepoMock.Object, _bookRepoMock.Object, _mapperMock.Object);
    }

    private ReturnBookCommand CreateCommand(int transactionId = 1, int returnedById = 1, string? notes = null)
    {
        return new ReturnBookCommand(new ReturnBookDto
        {
            TransactionId = transactionId,
            ReturnedById = returnedById,
            Notes = notes
        });
    }

    [Fact]
    public async Task Handle_TransactionNotFound_ThrowsKeyNotFoundException()
    {
        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync((BorrowingTransaction?)null);

        var act = () => _handler.Handle(CreateCommand(), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Transaction not found");
    }

    [Fact]
    public async Task Handle_AlreadyReturned_ThrowsInvalidOperationException()
    {
        var transaction = new BorrowingTransaction { Id = 1, Status = "Returned" };
        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync(transaction);

        var act = () => _handler.Handle(CreateCommand(), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Book already returned");
    }

    [Fact]
    public async Task Handle_BookNotFound_ThrowsKeyNotFoundException()
    {
        var transaction = new BorrowingTransaction { Id = 1, Status = "Borrowed", BookId = 5 };
        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync(transaction);
        _bookRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Book?)null);

        var act = () => _handler.Handle(CreateCommand(), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Book not found");
    }

    [Fact]
    public async Task Handle_HappyPath_SetsReturnedAndBookIn()
    {
        var transaction = new BorrowingTransaction
        {
            Id = 1, Status = "Borrowed", BookId = 1, Notes = "Original notes"
        };
        var book = new Book { Id = 1, Title = "Test", Status = BookStatus.Out };
        var transactionDto = new BorrowingTransactionDto { Id = 1 };

        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync(transaction);
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync(transaction);
        _mapperMock.Setup(m => m.Map<BorrowingTransactionDto>(transaction)).Returns(transactionDto);

        var result = await _handler.Handle(CreateCommand(), default);

        result.Should().Be(transactionDto);
        transaction.Status.Should().Be("Returned");
        transaction.ReturnDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        book.Status.Should().Be(BookStatus.In);
        _bookRepoMock.Verify(r => r.UpdateAsync(book), Times.Once);
        _borrowingRepoMock.Verify(r => r.UpdateAsync(transaction), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNotes_PreservesNewNotes()
    {
        var transaction = new BorrowingTransaction
        {
            Id = 1, Status = "Borrowed", BookId = 1, Notes = "Original notes"
        };
        var book = new Book { Id = 1, Status = BookStatus.Out };

        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync(transaction);
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync(transaction);
        _mapperMock.Setup(m => m.Map<BorrowingTransactionDto>(It.IsAny<BorrowingTransaction>())).Returns(new BorrowingTransactionDto());

        await _handler.Handle(CreateCommand(notes: "New notes"), default);

        transaction.Notes.Should().Be("New notes");
    }

    [Fact]
    public async Task Handle_WithoutNotes_KeepsOriginalNotes()
    {
        var transaction = new BorrowingTransaction
        {
            Id = 1, Status = "Borrowed", BookId = 1, Notes = "Original notes"
        };
        var book = new Book { Id = 1, Status = BookStatus.Out };

        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync(transaction);
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync(transaction);
        _mapperMock.Setup(m => m.Map<BorrowingTransactionDto>(It.IsAny<BorrowingTransaction>())).Returns(new BorrowingTransactionDto());

        await _handler.Handle(CreateCommand(notes: null), default);

        transaction.Notes.Should().Be("Original notes");
    }
}

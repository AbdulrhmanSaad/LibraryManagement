using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using FluentAssertions;
using LibraryManagement.Core.Commands.Borrowing;
using LibraryManagement.Core.Commands.Borrowing.Handlers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.UnitTests.Handlers;

public class BorrowBookCommandHandlerTests
{
    private readonly Mock<IBorrowingRepository> _borrowingRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IGenericRepository<Member>> _memberRepoMock;
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly BorrowBookCommandHandler _handler;

    public BorrowBookCommandHandlerTests()
    {
        _borrowingRepoMock = new Mock<IBorrowingRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _memberRepoMock = new Mock<IGenericRepository<Member>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            Mock.Of<IUserStore<AppUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _mapperMock = new Mock<IMapper>();

        _handler = new BorrowBookCommandHandler(
            _borrowingRepoMock.Object, _bookRepoMock.Object,
            _memberRepoMock.Object, _userManagerMock.Object, _mapperMock.Object);
    }

    private BorrowBookCommand CreateCommand(int bookId = 1, int memberId = 1, int borrowedById = 1, int borrowDays = 14, DateTime? borrowDate = null)
    {
        return new BorrowBookCommand(new CreateBorrowingDto
        {
            BookId = bookId,
            MemberId = memberId,
            BorrowedById = borrowedById,
            BorrowDays = borrowDays,
            BorrowDate = borrowDate
        });
    }

    [Fact]
    public async Task Handle_BookNotFound_ThrowsKeyNotFoundException()
    {
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Book?)null);

        var act = () => _handler.Handle(CreateCommand(), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Book not found");
    }

    [Fact]
    public async Task Handle_BookStatusOut_ThrowsInvalidOperationException()
    {
        var book = new Book { Id = 1, Title = "Test", Status = BookStatus.Out };
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);

        var act = () => _handler.Handle(CreateCommand(), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Book is not available for borrowing");
    }

    [Fact]
    public async Task Handle_MemberNotFound_ThrowsKeyNotFoundException()
    {
        var book = new Book { Id = 1, Title = "Test", Status = BookStatus.In };
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _memberRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Member?)null);

        var act = () => _handler.Handle(CreateCommand(), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Member not found");
    }

    [Fact]
    public async Task Handle_MemberInactive_ThrowsInvalidOperationException()
    {
        var book = new Book { Id = 1, Title = "Test", Status = BookStatus.In };
        var member = new Member { Id = 1, FirstName = "Bob", IsActive = false };
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _memberRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);

        var act = () => _handler.Handle(CreateCommand(), default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Member account is inactive");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        var book = new Book { Id = 1, Title = "Test", Status = BookStatus.In };
        var member = new Member { Id = 1, FirstName = "Alice", IsActive = true };
        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _memberRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync((AppUser?)null);

        var act = () => _handler.Handle(CreateCommand(), default);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("User not found");
    }

    [Fact]
    public async Task Handle_HappyPath_CreatesTransactionAndSetsBookOut()
    {
        var book = new Book { Id = 1, Title = "Test", Status = BookStatus.In };
        var member = new Member { Id = 1, FirstName = "Alice", IsActive = true };
        var user = new AppUser { Id = 1, UserName = "staff" };
        var createdTransaction = new BorrowingTransaction { Id = 42 };
        var dto = new BorrowingTransactionDto { Id = 42 };

        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _memberRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _borrowingRepoMock.Setup(r => r.AddAsync(It.IsAny<BorrowingTransaction>()))
            .Callback<BorrowingTransaction>(t => t.Id = 42)
            .ReturnsAsync(createdTransaction);
        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(42)).ReturnsAsync(createdTransaction);
        _mapperMock.Setup(m => m.Map<BorrowingTransactionDto>(createdTransaction)).Returns(dto);

        var result = await _handler.Handle(CreateCommand(), default);

        result.Should().Be(dto);
        book.Status.Should().Be(BookStatus.Out);
        _bookRepoMock.Verify(r => r.UpdateAsync(book), Times.Once);
        _borrowingRepoMock.Verify(r => r.AddAsync(It.IsAny<BorrowingTransaction>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCustomBorrowDate_CalculatesDueDateFromBorrowDate()
    {
        var borrowDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var book = new Book { Id = 1, Title = "Test", Status = BookStatus.In };
        var member = new Member { Id = 1, FirstName = "Alice", IsActive = true };
        var user = new AppUser { Id = 1, UserName = "staff" };

        _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _memberRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);
        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _borrowingRepoMock.Setup(r => r.AddAsync(It.IsAny<BorrowingTransaction>())).ReturnsAsync(new BorrowingTransaction { Id = 1 });
        _borrowingRepoMock.Setup(r => r.GetTransactionWithDetailsAsync(1)).ReturnsAsync(new BorrowingTransaction { Id = 1 });
        _mapperMock.Setup(m => m.Map<BorrowingTransactionDto>(It.IsAny<BorrowingTransaction>())).Returns(new BorrowingTransactionDto());

        await _handler.Handle(CreateCommand(borrowDays: 21, borrowDate: borrowDate), default);

        _borrowingRepoMock.Verify(r => r.AddAsync(It.Is<BorrowingTransaction>(t =>
            t.BorrowDate == borrowDate && t.DueDate == borrowDate.AddDays(21))), Times.Once);
    }
}

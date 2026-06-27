namespace LibraryManagement.Core.DTOs;

public class BorrowingTransactionDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookISBN { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string MemberNumber { get; set; } = string.Empty;
    public int BorrowedById { get; set; }
    public string BorrowedByName { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateBorrowingDto
{
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public int BorrowedById { get; set; }
    public DateTime? BorrowDate { get; set; }
    public int BorrowDays { get; set; } = 14;
    public string? Notes { get; set; }
}

public class ReturnBookDto
{
    public int TransactionId { get; set; }
    public int ReturnedById { get; set; }
    public string? Notes { get; set; }
}

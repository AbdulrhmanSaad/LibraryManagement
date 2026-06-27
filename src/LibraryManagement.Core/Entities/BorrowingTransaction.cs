namespace LibraryManagement.Core.Entities;

public class BorrowingTransaction
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public int BorrowedById { get; set; }
    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = "Borrowed";
    public string? Notes { get; set; }

    public Book Book { get; set; } = null!;
    public Member Member { get; set; } = null!;
    public AppUser BorrowedBy { get; set; } = null!;
}

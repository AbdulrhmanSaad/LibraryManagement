namespace LibraryManagement.Core.Entities;

public class Book
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Edition { get; set; }
    public int? PublicationYear { get; set; }
    public string? Summary { get; set; }
    public string? CoverImageUrl { get; set; }
    public BookStatus Status { get; set; } = BookStatus.In;
    public int? PageCount { get; set; }
    public string Language { get; set; } = "English";
    public int? PublisherId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Publisher? Publisher { get; set; }
    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();
}

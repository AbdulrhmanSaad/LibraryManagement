namespace LibraryManagement.Core.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Edition { get; set; }
    public int? PublicationYear { get; set; }
    public string? Summary { get; set; }
    public string? CoverImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? PageCount { get; set; }
    public string Language { get; set; } = string.Empty;
    public int? PublisherId { get; set; }
    public string? PublisherName { get; set; }
    public List<string> Authors { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateBookDto
{
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Edition { get; set; }
    public int? PublicationYear { get; set; }
    public string? Summary { get; set; }
    public string? CoverImageUrl { get; set; }
    public int? PageCount { get; set; }
    public string Language { get; set; } = "English";
    public int? PublisherId { get; set; }
    public List<int> AuthorIds { get; set; } = new();
    public List<int> CategoryIds { get; set; } = new();
}

public class UpdateBookDto
{
    public string? ISBN { get; set; }
    public string? Title { get; set; }
    public string? Edition { get; set; }
    public int? PublicationYear { get; set; }
    public string? Summary { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Status { get; set; }
    public int? PageCount { get; set; }
    public string? Language { get; set; }
    public int? PublisherId { get; set; }
    public List<int>? AuthorIds { get; set; }
    public List<int>? CategoryIds { get; set; }
}

public class BookSearchDto
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Category { get; set; }
    public string? Language { get; set; }
    public string? Status { get; set; }
}

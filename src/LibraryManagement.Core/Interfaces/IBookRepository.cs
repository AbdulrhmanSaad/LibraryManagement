using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<IEnumerable<Book>> SearchBooksAsync(string? title, string? author, string? category);
    Task<IEnumerable<Book>> GetBooksByStatusAsync(BookStatus status);
    Task<Book?> GetBookWithDetailsAsync(int id);
    Task<IEnumerable<Book>> GetAllWithDetailsAsync();
}

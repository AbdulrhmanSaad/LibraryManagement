using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context) { }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string? title, string? author, string? category)
    {
        var query = _dbSet
            .Include(b => b.Authors)
            .Include(b => b.Categories)
            .Include(b => b.Publisher)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(b => b.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(b => b.Authors.Any(a =>
                (a.FirstName + " " + a.LastName).Contains(author)));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(b => b.Categories.Any(c => c.Name.Contains(category)));

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByStatusAsync(BookStatus status)
    {
        return await _dbSet
            .Include(b => b.Authors)
            .Include(b => b.Categories)
            .Include(b => b.Publisher)
            .Where(b => b.Status == status)
            .ToListAsync();
    }

    public async Task<Book?> GetBookWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Authors)
            .Include(b => b.Categories)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Book>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(b => b.Authors)
            .Include(b => b.Categories)
            .Include(b => b.Publisher)
            .ToListAsync();
    }
}

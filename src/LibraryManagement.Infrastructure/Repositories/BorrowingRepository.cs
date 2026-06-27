using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.Infrastructure.Repositories;

public class BorrowingRepository : GenericRepository<BorrowingTransaction>, IBorrowingRepository
{
    public BorrowingRepository(LibraryDbContext context) : base(context) { }

    public async Task<IEnumerable<BorrowingTransaction>> GetByMemberIdAsync(int memberId)
    {
        return await _dbSet
            .Include(bt => bt.Book)
            .Include(bt => bt.Member)
            .Include(bt => bt.BorrowedBy)
            .Where(bt => bt.MemberId == memberId)
            .OrderByDescending(bt => bt.BorrowDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BorrowingTransaction>> GetByBookIdAsync(int bookId)
    {
        return await _dbSet
            .Include(bt => bt.Book)
            .Include(bt => bt.Member)
            .Include(bt => bt.BorrowedBy)
            .Where(bt => bt.BookId == bookId)
            .OrderByDescending(bt => bt.BorrowDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<BorrowingTransaction>> GetActiveBorrowingsAsync()
    {
        return await _dbSet
            .Include(bt => bt.Book)
            .Include(bt => bt.Member)
            .Include(bt => bt.BorrowedBy)
            .Where(bt => bt.Status == "Borrowed" || bt.Status == "Overdue")
            .OrderByDescending(bt => bt.BorrowDate)
            .ToListAsync();
    }

    public async Task<BorrowingTransaction?> GetTransactionWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(bt => bt.Book)
            .Include(bt => bt.Member)
            .Include(bt => bt.BorrowedBy)
            .FirstOrDefaultAsync(bt => bt.Id == id);
    }
}

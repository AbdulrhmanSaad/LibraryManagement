using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces;

public interface IBorrowingRepository : IGenericRepository<BorrowingTransaction>
{
    Task<IEnumerable<BorrowingTransaction>> GetByMemberIdAsync(int memberId);
    Task<IEnumerable<BorrowingTransaction>> GetByBookIdAsync(int bookId);
    Task<IEnumerable<BorrowingTransaction>> GetActiveBorrowingsAsync();
    Task<BorrowingTransaction?> GetTransactionWithDetailsAsync(int id);
}

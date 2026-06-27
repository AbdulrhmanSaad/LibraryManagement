using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Infrastructure.Data;

public class LibraryDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BorrowingTransaction> BorrowingTransactions => Set<BorrowingTransaction>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Member>(e =>
        {
            e.HasIndex(m => m.MemberNumber).IsUnique();
            e.HasIndex(m => m.Email).IsUnique();
        });

        modelBuilder.Entity<Book>(e =>
        {
            e.HasIndex(b => b.ISBN).IsUnique();
            e.Property(b => b.Status).HasMaxLength(10).HasConversion<string>();
            e.Property(b => b.Language).HasMaxLength(50);

            e.HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Author>(e =>
        {
            e.HasIndex(a => new { a.FirstName, a.LastName });
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BorrowingTransaction>(e =>
        {
            e.Property(bt => bt.Status).HasMaxLength(20);

            e.HasOne(bt => bt.Book)
                .WithMany(b => b.BorrowingTransactions)
                .HasForeignKey(bt => bt.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(bt => bt.Member)
                .WithMany(m => m.BorrowingTransactions)
                .HasForeignKey(bt => bt.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(bt => bt.BorrowedBy)
                .WithMany(u => u.ProcessedTransactions)
                .HasForeignKey(bt => bt.BorrowedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ActivityLog>(e =>
        {
            e.HasOne(al => al.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Authors)
            .WithMany(a => a.Books)
            .UsingEntity(j => j.ToTable("BookAuthors"));

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Categories)
            .WithMany(c => c.Books)
            .UsingEntity(j => j.ToTable("BookCategories"));

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasOne(rt => rt.AppUser)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(rt => rt.Token).IsUnique();
        });
    }
}

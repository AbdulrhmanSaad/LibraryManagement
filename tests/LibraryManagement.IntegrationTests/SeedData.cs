using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.IntegrationTests;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;
        var dbContext = sp.GetRequiredService<LibraryDbContext>();
        var userManager = sp.GetRequiredService<UserManager<AppUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<int>>>();

        if (dbContext.Set<Author>().Any())
            return;

        // Seed roles
        await roleManager.CreateAsync(new IdentityRole<int>("Administrator"));
        await roleManager.CreateAsync(new IdentityRole<int>("Librarian"));
        await roleManager.CreateAsync(new IdentityRole<int>("Staff"));

        // Seed users
        var admin = new AppUser
        {
            UserName = "admin",
            Email = "admin@library.com",
            FullName = "Admin User",
            IsActive = true
        };
        await userManager.CreateAsync(admin, "Password123");
        await userManager.AddToRoleAsync(admin, "Administrator");

        var librarian1 = new AppUser
        {
            UserName = "librarian1",
            Email = "librarian1@library.com",
            FullName = "Alice Johnson",
            IsActive = true
        };
        await userManager.CreateAsync(librarian1, "Password123");
        await userManager.AddToRoleAsync(librarian1, "Librarian");

        var staff1 = new AppUser
        {
            UserName = "staff1",
            Email = "staff1@library.com",
            FullName = "Carol Davis",
            IsActive = true
        };
        await userManager.CreateAsync(staff1, "Password123");
        await userManager.AddToRoleAsync(staff1, "Staff");

        // Seed authors
        var rowling = new Author { FirstName = "J.K.", LastName = "Rowling" };
        var orwell = new Author { FirstName = "George", LastName = "Orwell" };
        var austen = new Author { FirstName = "Jane", LastName = "Austen" };
        dbContext.Set<Author>().AddRange(rowling, orwell, austen);
        await dbContext.SaveChangesAsync();

        // Seed categories
        var fiction = new Category { Name = "Fiction" };
        var nonFiction = new Category { Name = "Non-Fiction" };
        dbContext.Set<Category>().AddRange(fiction, nonFiction);
        await dbContext.SaveChangesAsync();

        // Seed publishers
        var penguin = new Publisher { Name = "Penguin Books" };
        var harpercollins = new Publisher { Name = "HarperCollins" };
        dbContext.Set<Publisher>().AddRange(penguin, harpercollins);
        await dbContext.SaveChangesAsync();

        // Seed books
        var harryPotter = new Book
        {
            ISBN = "978-0-7475-3269-9",
            Title = "Harry Potter",
            Status = BookStatus.In,
            Language = "English"
        };
        harryPotter.Authors.Add(rowling);
        harryPotter.Categories.Add(fiction);

        var book1984 = new Book
        {
            ISBN = "978-0-452-28423-4",
            Title = "1984",
            Status = BookStatus.In,
            Language = "English"
        };
        book1984.Authors.Add(orwell);
        book1984.Categories.Add(nonFiction);

        var prideAndPrejudice = new Book
        {
            ISBN = "978-0-141-03936-9",
            Title = "Pride and Prejudice",
            Status = BookStatus.Out,
            Language = "English"
        };
        prideAndPrejudice.Authors.Add(austen);
        prideAndPrejudice.Categories.Add(fiction);

        dbContext.Set<Book>().AddRange(harryPotter, book1984, prideAndPrejudice);
        await dbContext.SaveChangesAsync();

        // Seed members
        var aliceSmith = new Member
        {
            MemberNumber = "MEM-001",
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@library.com",
            IsActive = true
        };
        var bobJones = new Member
        {
            MemberNumber = "MEM-002",
            FirstName = "Bob",
            LastName = "Jones",
            Email = "bob@library.com",
            IsActive = false
        };
        dbContext.Set<Member>().AddRange(aliceSmith, bobJones);
        await dbContext.SaveChangesAsync();

        // Seed borrowing transaction for Pride and Prejudice
        var transaction = new BorrowingTransaction
        {
            BookId = prideAndPrejudice.Id,
            MemberId = aliceSmith.Id,
            BorrowedById = librarian1.Id,
            BorrowDate = DateTime.UtcNow.AddDays(-14),
            DueDate = DateTime.UtcNow,
            Status = "Borrowed"
        };
        dbContext.Set<BorrowingTransaction>().Add(transaction);
        await dbContext.SaveChangesAsync();
    }
}

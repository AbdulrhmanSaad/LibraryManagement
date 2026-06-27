using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var sp = scope.ServiceProvider;
        var context = sp.GetRequiredService<LibraryDbContext>();
        var userManager = sp.GetRequiredService<UserManager<AppUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await SeedRoles(roleManager);
        await SeedUsers(userManager);
        await SeedPublishers(context);
        await SeedCategories(context);
        await SeedAuthors(context);
        await SeedBooks(context);
        await SeedMembers(context);
        await SeedBorrowingTransactions(context);
    }

    private static async Task SeedRoles(RoleManager<IdentityRole<int>> roleManager)
    {
        var roles = new[] { "Administrator", "Librarian", "Staff" };
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
    }

    private static async Task SeedUsers(UserManager<AppUser> userManager)
    {
        var users = new (string Username, string Email, string FullName, string Role, string Password)[]
        {
            ("admin", "admin@library.com", "System Administrator", "Administrator", "Password123"),
            ("librarian1", "librarian1@library.com", "Alice Johnson", "Librarian", "Password123"),
            ("librarian2", "librarian2@library.com", "Bob Williams", "Librarian", "Password123"),
            ("staff1", "staff1@library.com", "Carol Davis", "Staff", "Password123"),
            ("staff2", "staff2@library.com", "David Brown", "Staff", "Password123"),
        };

        foreach (var (username, email, fullName, role, password) in users)
        {
            if (await userManager.FindByNameAsync(username) != null) continue;

            var user = new AppUser
            {
                UserName = username,
                Email = email,
                FullName = fullName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }

    private static async Task SeedPublishers(LibraryDbContext context)
    {
        if (context.Publishers.Any()) return;

        context.Publishers.AddRange(
            new Publisher { Name = "Penguin Random House", Address = "1745 Broadway, New York, NY 10019", Phone = "+1-212-782-9000", Email = "info@penguinrandomhouse.com", Website = "https://www.penguinrandomhouse.com" },
            new Publisher { Name = "HarperCollins", Address = "195 Broadway, New York, NY 10007", Phone = "+1-212-207-7000", Email = "contact@harpercollins.com", Website = "https://www.harpercollins.com" },
            new Publisher { Name = "Simon & Schuster", Address = "1230 Avenue of the Americas, New York, NY 10020", Phone = "+1-212-698-7000", Email = "info@simonandschuster.com", Website = "https://www.simonandschuster.com" },
            new Publisher { Name = "O'Reilly Media", Address = "1005 Gravenstein Hwy N, Sebastopol, CA 95472", Phone = "+1-707-827-7000", Email = "info@oreilly.com", Website = "https://www.oreilly.com" },
            new Publisher { Name = "Oxford University Press", Address = "Great Clarendon Street, Oxford, OX2 6DP, UK", Phone = "+44-1865-353000", Email = "info@oup.com", Website = "https://global.oup.com" }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedCategories(LibraryDbContext context)
    {
        if (context.Categories.Any()) return;

        var fiction = new Category { Name = "Fiction", Description = "Fictional literature" };
        var nonFiction = new Category { Name = "Non-Fiction", Description = "Non-fictional literature" };
        var scienceTech = new Category { Name = "Science & Technology", Description = "Scientific and technical books" };

        context.Categories.AddRange(fiction, nonFiction, scienceTech);
        await context.SaveChangesAsync();

        context.Categories.AddRange(
            new Category { Name = "Classic Literature", Description = "Classic works of literature", ParentCategoryId = fiction.Id },
            new Category { Name = "Science Fiction", Description = "Science fiction books", ParentCategoryId = fiction.Id },
            new Category { Name = "Fantasy", Description = "Fantasy books", ParentCategoryId = fiction.Id },
            new Category { Name = "Mystery & Thriller", Description = "Mystery and thriller books", ParentCategoryId = fiction.Id },
            new Category { Name = "History", Description = "Historical books", ParentCategoryId = nonFiction.Id },
            new Category { Name = "Biography", Description = "Biographical works", ParentCategoryId = nonFiction.Id },
            new Category { Name = "Self-Help", Description = "Self-help and personal development", ParentCategoryId = nonFiction.Id },
            new Category { Name = "Computer Science", Description = "Computer science and programming", ParentCategoryId = scienceTech.Id },
            new Category { Name = "Engineering", Description = "Engineering books", ParentCategoryId = scienceTech.Id },
            new Category { Name = "Mathematics", Description = "Mathematics books", ParentCategoryId = scienceTech.Id }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedAuthors(LibraryDbContext context)
    {
        if (context.Authors.Any()) return;

        context.Authors.AddRange(
            new Author { FirstName = "George", LastName = "Orwell", Biography = "English novelist, essayist, journalist and critic", BirthDate = new DateTime(1903, 6, 25) },
            new Author { FirstName = "Jane", LastName = "Austen", Biography = "English novelist known for six major novels", BirthDate = new DateTime(1775, 12, 16) },
            new Author { FirstName = "J.R.R.", LastName = "Tolkien", Biography = "English writer, poet, philologist and academic", BirthDate = new DateTime(1892, 1, 3) },
            new Author { FirstName = "Isaac", LastName = "Asimov", Biography = "American writer and professor of biochemistry", BirthDate = new DateTime(1920, 1, 2) },
            new Author { FirstName = "Stephen", LastName = "King", Biography = "American author of horror, supernatural fiction and fantasy", BirthDate = new DateTime(1947, 9, 21) },
            new Author { FirstName = "Yuval Noah", LastName = "Harari", Biography = "Israeli public intellectual, historian and professor", BirthDate = new DateTime(1976, 2, 24) },
            new Author { FirstName = "Robert C.", LastName = "Martin", Biography = "American software engineer and author", BirthDate = new DateTime(1952, 12, 5) },
            new Author { FirstName = "Andrew", LastName = "Tanenbaum", Biography = "American-Dutch computer scientist and professor", BirthDate = new DateTime(1944, 3, 16) },
            new Author { FirstName = "Donald", LastName = "Knuth", Biography = "American computer scientist and mathematician", BirthDate = new DateTime(1938, 1, 10) },
            new Author { FirstName = "Martin", LastName = "Fowler", Biography = "British software engineer and author", BirthDate = new DateTime(1963, 12, 18) }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedBooks(LibraryDbContext context)
    {
        if (context.Books.Any()) return;

        var pub = context.Publishers.ToDictionary(p => p.Name, p => p.Id);
        var authors = context.Authors.ToDictionary(a => a.LastName, a => a);
        var cat = context.Categories.ToDictionary(c => c.Name, c => c);

        var books = new List<Book>
        {
            new() { ISBN = "978-0451524935", Title = "1984", Edition = "Signet Classics", PublicationYear = 1950, Summary = "A dystopian novel set in a totalitarian society ruled by Big Brother", Status = BookStatus.In, PageCount = 328, Language = "English", PublisherId = pub["Penguin Random House"] },
            new() { ISBN = "978-0141439518", Title = "Pride and Prejudice", Edition = "Penguin Classics", PublicationYear = 1813, Summary = "A romantic novel following Elizabeth Bennet", Status = BookStatus.In, PageCount = 432, Language = "English", PublisherId = pub["Penguin Random House"] },
            new() { ISBN = "978-0547928227", Title = "The Hobbit", Edition = "Mariner Books", PublicationYear = 1937, Summary = "A fantasy novel about the journey of Bilbo Baggins", Status = BookStatus.In, PageCount = 310, Language = "English", PublisherId = pub["HarperCollins"] },
            new() { ISBN = "978-0553293357", Title = "Foundation", Edition = "Bantam Books", PublicationYear = 1951, Summary = "A science fiction novel about the fall and rise of a galactic empire", Status = BookStatus.Out, PageCount = 244, Language = "English", PublisherId = pub["Penguin Random House"] },
            new() { ISBN = "978-1501142970", Title = "The Institute", Edition = "Scribner", PublicationYear = 2019, Summary = "A horror novel about children with special abilities", Status = BookStatus.In, PageCount = 576, Language = "English", PublisherId = pub["Simon & Schuster"] },
            new() { ISBN = "978-0062316097", Title = "Sapiens: A Brief History of Humankind", Edition = "Harper Perennial", PublicationYear = 2015, Summary = "A historical overview of the human species", Status = BookStatus.In, PageCount = 464, Language = "English", PublisherId = pub["HarperCollins"] },
            new() { ISBN = "978-0132350884", Title = "Clean Code: A Handbook of Agile Software Craftsmanship", Edition = "1st", PublicationYear = 2008, Summary = "A guide to writing clean, maintainable code", Status = BookStatus.Out, PageCount = 464, Language = "English", PublisherId = pub["O'Reilly Media"] },
            new() { ISBN = "978-0133594141", Title = "Modern Operating Systems", Edition = "4th", PublicationYear = 2014, Summary = "Comprehensive coverage of operating system concepts", Status = BookStatus.In, PageCount = 1136, Language = "English", PublisherId = pub["O'Reilly Media"] },
            new() { ISBN = "978-0201896831", Title = "The Art of Computer Programming", Edition = "3rd", PublicationYear = 1997, Summary = "A comprehensive treatise on computer programming algorithms", Status = BookStatus.In, PageCount = 672, Language = "English", PublisherId = pub["O'Reilly Media"] },
            new() { ISBN = "978-0321125217", Title = "Domain-Driven Design", Edition = "1st", PublicationYear = 2003, Summary = "A guide to designing complex software systems", Status = BookStatus.In, PageCount = 560, Language = "English", PublisherId = pub["O'Reilly Media"] },
            new() { ISBN = "978-0140449266", Title = "War and Peace", Edition = "Penguin Classics", PublicationYear = 1869, Summary = "An epic novel about Russian society during the Napoleonic Era", Status = BookStatus.In, PageCount = 1392, Language = "English", PublisherId = pub["Penguin Random House"] },
            new() { ISBN = "978-0261102385", Title = "The Fellowship of the Ring", Edition = "HarperCollins", PublicationYear = 1954, Summary = "First volume of The Lord of the Rings", Status = BookStatus.Out, PageCount = 432, Language = "English", PublisherId = pub["HarperCollins"] }
        };

        var bookAuthors = new Dictionary<string, string[]>
        {
            ["1984"] = ["Orwell"], ["Pride and Prejudice"] = ["Austen"], ["The Hobbit"] = ["Tolkien"],
            ["Foundation"] = ["Asimov"], ["The Institute"] = ["King"], ["Sapiens: A Brief History of Humankind"] = ["Harari"],
            ["Clean Code: A Handbook of Agile Software Craftsmanship"] = ["Martin"], ["Modern Operating Systems"] = ["Tanenbaum"],
            ["The Art of Computer Programming"] = ["Knuth"], ["Domain-Driven Design"] = ["Fowler"],
            ["War and Peace"] = ["Austen"], ["The Fellowship of the Ring"] = ["Tolkien"]
        };

        var bookCategories = new Dictionary<string, string[]>
        {
            ["1984"] = ["Classic Literature", "Science Fiction"], ["Pride and Prejudice"] = ["Classic Literature"],
            ["The Hobbit"] = ["Fantasy"], ["Foundation"] = ["Science Fiction"],
            ["The Institute"] = ["Mystery & Thriller"], ["Sapiens: A Brief History of Humankind"] = ["History"],
            ["Clean Code: A Handbook of Agile Software Craftsmanship"] = ["Computer Science"],
            ["Modern Operating Systems"] = ["Computer Science"],
            ["The Art of Computer Programming"] = ["Computer Science", "Mathematics"],
            ["Domain-Driven Design"] = ["Computer Science"],
            ["War and Peace"] = ["Classic Literature", "History"], ["The Fellowship of the Ring"] = ["Fantasy"]
        };

        foreach (var book in books)
        {
            if (bookAuthors.TryGetValue(book.Title, out var authorKeys))
                foreach (var key in authorKeys) book.Authors.Add(authors[key]);
            if (bookCategories.TryGetValue(book.Title, out var catKeys))
                foreach (var key in catKeys) book.Categories.Add(cat[key]);
            context.Books.Add(book);
        }
        await context.SaveChangesAsync();
    }

    private static async Task SeedMembers(LibraryDbContext context)
    {
        if (context.Members.Any()) return;

        context.Members.AddRange(
            new Member { MemberNumber = "MEM-20250101-1001", FirstName = "John", LastName = "Smith", Email = "john.smith@email.com", Phone = "+1-555-0101", Address = "123 Main St, Springfield, IL 62701", DateOfBirth = new DateTime(1990, 5, 15) },
            new Member { MemberNumber = "MEM-20250101-1002", FirstName = "Emma", LastName = "Watson", Email = "emma.watson@email.com", Phone = "+1-555-0102", Address = "456 Oak Ave, Portland, OR 97201", DateOfBirth = new DateTime(1988, 11, 22) },
            new Member { MemberNumber = "MEM-20250101-1003", FirstName = "Michael", LastName = "Brown", Email = "michael.brown@email.com", Phone = "+1-555-0103", Address = "789 Pine Rd, Austin, TX 73301", DateOfBirth = new DateTime(1995, 3, 8) },
            new Member { MemberNumber = "MEM-20250101-1004", FirstName = "Sarah", LastName = "Wilson", Email = "sarah.wilson@email.com", Phone = "+1-555-0104", Address = "321 Elm St, Denver, CO 80201", DateOfBirth = new DateTime(2000, 7, 30) },
            new Member { MemberNumber = "MEM-20250101-1005", FirstName = "James", LastName = "Taylor", Email = "james.taylor@email.com", Phone = "+1-555-0105", Address = "654 Maple Dr, Boston, MA 02101", DateOfBirth = new DateTime(1985, 9, 12) }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedBorrowingTransactions(LibraryDbContext context)
    {
        if (context.BorrowingTransactions.Any()) return;

        var librarian = context.Users.First(u => u.UserName == "librarian1");
        var staff = context.Users.First(u => u.UserName == "staff1");
        var books = context.Books.ToDictionary(b => b.Title, b => b.Id);
        var members = context.Members.ToDictionary(m => m.LastName, m => m.Id);

        context.BorrowingTransactions.AddRange(
            new BorrowingTransaction { BookId = books["Foundation"], MemberId = members["Smith"], BorrowedById = librarian.Id, BorrowDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc), DueDate = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), Status = "Borrowed" },
            new BorrowingTransaction { BookId = books["Clean Code: A Handbook of Agile Software Craftsmanship"], MemberId = members["Watson"], BorrowedById = librarian.Id, BorrowDate = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc), DueDate = new DateTime(2026, 6, 19, 0, 0, 0, DateTimeKind.Utc), Status = "Borrowed" },
            new BorrowingTransaction { BookId = books["The Fellowship of the Ring"], MemberId = members["Brown"], BorrowedById = staff.Id, BorrowDate = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc), DueDate = new DateTime(2026, 6, 24, 0, 0, 0, DateTimeKind.Utc), Status = "Borrowed" },
            new BorrowingTransaction { BookId = books["1984"], MemberId = members["Wilson"], BorrowedById = librarian.Id, BorrowDate = new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc), DueDate = new DateTime(2026, 6, 3, 0, 0, 0, DateTimeKind.Utc), ReturnDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc), Status = "Returned" },
            new BorrowingTransaction { BookId = books["The Hobbit"], MemberId = members["Taylor"], BorrowedById = staff.Id, BorrowDate = new DateTime(2026, 5, 25, 0, 0, 0, DateTimeKind.Utc), DueDate = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), ReturnDate = new DateTime(2026, 6, 5, 0, 0, 0, DateTimeKind.Utc), Status = "Returned" }
        );
        await context.SaveChangesAsync();
    }
}

-- Library Management System Sample Data
-- Use the DataSeeder in LibraryManagement.Infrastructure for the authoritative seed.
-- Identity tables (AspNetUsers, AspNetRoles, AspNetUserRoles) are populated by the app's DataSeeder at startup.

USE LibraryManagementDb;
GO

-- Publishers
IF NOT EXISTS (SELECT 1 FROM Publishers)
BEGIN
    INSERT INTO Publishers (Name, Address, Phone, Email, Website) VALUES
    ('Penguin Random House', '1745 Broadway, New York, NY 10019', '+1-212-782-9000', 'info@penguinrandomhouse.com', 'https://www.penguinrandomhouse.com'),
    ('HarperCollins', '195 Broadway, New York, NY 10007', '+1-212-207-7000', 'contact@harpercollins.com', 'https://www.harpercollins.com'),
    ('Simon & Schuster', '1230 Avenue of the Americas, New York, NY 10020', '+1-212-698-7000', 'info@simonandschuster.com', 'https://www.simonandschuster.com'),
    ('O''Reilly Media', '1005 Gravenstein Hwy N, Sebastopol, CA 95472', '+1-707-827-7000', 'info@oreilly.com', 'https://www.oreilly.com'),
    ('Oxford University Press', 'Great Clarendon Street, Oxford, OX2 6DP, UK', '+44-1865-353000', 'info@oup.com', 'https://global.oup.com');
END
GO

-- Categories (hierarchical)
IF NOT EXISTS (SELECT 1 FROM Categories)
BEGIN
    DECLARE @fiction INT, @nonFiction INT, @scienceTech INT;

    INSERT INTO Categories (Name, Description) VALUES ('Fiction', 'Fictional literature');
    SET @fiction = SCOPE_IDENTITY();

    INSERT INTO Categories (Name, Description) VALUES ('Non-Fiction', 'Non-fictional literature');
    SET @nonFiction = SCOPE_IDENTITY();

    INSERT INTO Categories (Name, Description) VALUES ('Science & Technology', 'Scientific and technical books');
    SET @scienceTech = SCOPE_IDENTITY();

    INSERT INTO Categories (Name, Description, ParentCategoryId) VALUES
    ('Classic Literature', 'Classic works of literature', @fiction),
    ('Science Fiction', 'Science fiction books', @fiction),
    ('Fantasy', 'Fantasy books', @fiction),
    ('Mystery & Thriller', 'Mystery and thriller books', @fiction),
    ('History', 'Historical books', @nonFiction),
    ('Biography', 'Biographical works', @nonFiction),
    ('Self-Help', 'Self-help and personal development', @nonFiction),
    ('Computer Science', 'Computer science and programming', @scienceTech),
    ('Engineering', 'Engineering books', @scienceTech),
    ('Mathematics', 'Mathematics books', @scienceTech);
END
GO

-- Authors
IF NOT EXISTS (SELECT 1 FROM Authors)
BEGIN
    INSERT INTO Authors (FirstName, LastName, Biography, BirthDate) VALUES
    ('George', 'Orwell', 'English novelist, essayist, journalist and critic', '1903-06-25'),
    ('Jane', 'Austen', 'English novelist known for six major novels', '1775-12-16'),
    ('J.R.R.', 'Tolkien', 'English writer, poet, philologist and academic', '1892-01-03'),
    ('Isaac', 'Asimov', 'American writer and professor of biochemistry', '1920-01-02'),
    ('Stephen', 'King', 'American author of horror, supernatural fiction and fantasy', '1947-09-21'),
    ('Yuval Noah', 'Harari', 'Israeli public intellectual, historian and professor', '1976-02-24'),
    ('Robert C.', 'Martin', 'American software engineer and author', '1952-12-05'),
    ('Andrew', 'Tanenbaum', 'American-Dutch computer scientist and professor', '1944-03-16'),
    ('Donald', 'Knuth', 'American computer scientist and mathematician', '1938-01-10'),
    ('Martin', 'Fowler', 'British software engineer and author', '1963-12-18');
END
GO

-- Books
IF NOT EXISTS (SELECT 1 FROM Books)
BEGIN
    DECLARE @pubPenguin INT, @pubHarper INT, @pubSimon INT, @pubOreilly INT;
    SELECT @pubPenguin = Id FROM Publishers WHERE Name = 'Penguin Random House';
    SELECT @pubHarper = Id FROM Publishers WHERE Name = 'HarperCollins';
    SELECT @pubSimon = Id FROM Publishers WHERE Name = 'Simon & Schuster';
    SELECT @pubOreilly = Id FROM Publishers WHERE Name = 'O''Reilly Media';

    INSERT INTO Books (ISBN, Title, Edition, PublicationYear, Summary, Status, PageCount, Language, PublisherId) VALUES
    ('978-0451524935', '1984', 'Signet Classics', 1950, 'A dystopian novel set in a totalitarian society ruled by Big Brother', 'In', 328, 'English', @pubPenguin),
    ('978-0141439518', 'Pride and Prejudice', 'Penguin Classics', 1813, 'A romantic novel following Elizabeth Bennet', 'In', 432, 'English', @pubPenguin),
    ('978-0547928227', 'The Hobbit', 'Mariner Books', 1937, 'A fantasy novel about the journey of Bilbo Baggins', 'In', 310, 'English', @pubHarper),
    ('978-0553293357', 'Foundation', 'Bantam Books', 1951, 'A science fiction novel about the fall and rise of a galactic empire', 'Out', 244, 'English', @pubPenguin),
    ('978-1501142970', 'The Institute', 'Scribner', 2019, 'A horror novel about children with special abilities', 'In', 576, 'English', @pubSimon),
    ('978-0062316097', 'Sapiens: A Brief History of Humankind', 'Harper Perennial', 2015, 'A historical overview of the human species', 'In', 464, 'English', @pubHarper),
    ('978-0132350884', 'Clean Code: A Handbook of Agile Software Craftsmanship', '1st', 2008, 'A guide to writing clean, maintainable code', 'Out', 464, 'English', @pubOreilly),
    ('978-0133594141', 'Modern Operating Systems', '4th', 2014, 'Comprehensive coverage of operating system concepts', 'In', 1136, 'English', @pubOreilly),
    ('978-0201896831', 'The Art of Computer Programming', '3rd', 1997, 'A comprehensive treatise on computer programming algorithms', 'In', 672, 'English', @pubOreilly),
    ('978-0321125217', 'Domain-Driven Design', '1st', 2003, 'A guide to designing complex software systems', 'In', 560, 'English', @pubOreilly),
    ('978-0140449266', 'War and Peace', 'Penguin Classics', 1869, 'An epic novel about Russian society during the Napoleonic Era', 'In', 1392, 'English', @pubPenguin),
    ('978-0261102385', 'The Fellowship of the Ring', 'HarperCollins', 1954, 'First volume of The Lord of the Rings', 'Out', 432, 'English', @pubHarper);
END
GO

-- BookAuthors (M:N)
IF NOT EXISTS (SELECT 1 FROM BookAuthors)
BEGIN
    DECLARE @bookId INT, @authorId INT;

    -- 1984 -> Orwell
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0451524935';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Orwell';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- Pride and Prejudice -> Austen
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0141439518';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Austen';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- The Hobbit -> Tolkien
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0547928227';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Tolkien';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- Foundation -> Asimov
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0553293357';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Asimov';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- The Institute -> King
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-1501142970';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'King';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- Sapiens -> Harari
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0062316097';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Harari';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- Clean Code -> Martin
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0132350884';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Martin';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- Modern Operating Systems -> Tanenbaum
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0133594141';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Tanenbaum';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- The Art of Computer Programming -> Knuth
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0201896831';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Knuth';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- Domain-Driven Design -> Fowler
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0321125217';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Fowler';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- War and Peace -> Austen
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0140449266';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Austen';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);

    -- The Fellowship of the Ring -> Tolkien
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0261102385';
    SELECT @authorId = Id FROM Authors WHERE LastName = 'Tolkien';
    INSERT INTO BookAuthors (AuthorsId, BooksId) VALUES (@authorId, @bookId);
END
GO

-- BookCategories (M:N)
IF NOT EXISTS (SELECT 1 FROM BookCategories)
BEGIN
    DECLARE @bookId INT, @catId INT;

    -- 1984 -> Classic Literature, Science Fiction
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0451524935';
    SELECT @catId = Id FROM Categories WHERE Name = 'Classic Literature'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);
    SELECT @catId = Id FROM Categories WHERE Name = 'Science Fiction'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- Pride and Prejudice -> Classic Literature
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0141439518';
    SELECT @catId = Id FROM Categories WHERE Name = 'Classic Literature'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- The Hobbit -> Fantasy
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0547928227';
    SELECT @catId = Id FROM Categories WHERE Name = 'Fantasy'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- Foundation -> Science Fiction
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0553293357';
    SELECT @catId = Id FROM Categories WHERE Name = 'Science Fiction'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- The Institute -> Mystery & Thriller
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-1501142970';
    SELECT @catId = Id FROM Categories WHERE Name = 'Mystery & Thriller'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- Sapiens -> History
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0062316097';
    SELECT @catId = Id FROM Categories WHERE Name = 'History'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- Clean Code -> Computer Science
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0132350884';
    SELECT @catId = Id FROM Categories WHERE Name = 'Computer Science'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- Modern Operating Systems -> Computer Science
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0133594141';
    SELECT @catId = Id FROM Categories WHERE Name = 'Computer Science'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- The Art of Computer Programming -> Computer Science, Mathematics
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0201896831';
    SELECT @catId = Id FROM Categories WHERE Name = 'Computer Science'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);
    SELECT @catId = Id FROM Categories WHERE Name = 'Mathematics'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- Domain-Driven Design -> Computer Science
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0321125217';
    SELECT @catId = Id FROM Categories WHERE Name = 'Computer Science'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- War and Peace -> Classic Literature, History
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0140449266';
    SELECT @catId = Id FROM Categories WHERE Name = 'Classic Literature'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);
    SELECT @catId = Id FROM Categories WHERE Name = 'History'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);

    -- The Fellowship of the Ring -> Fantasy
    SELECT @bookId = Id FROM Books WHERE ISBN = '978-0261102385';
    SELECT @catId = Id FROM Categories WHERE Name = 'Fantasy'; INSERT INTO BookCategories (BooksId, CategoriesId) VALUES (@bookId, @catId);
END
GO

-- Members
IF NOT EXISTS (SELECT 1 FROM Members)
BEGIN
    INSERT INTO Members (MemberNumber, FirstName, LastName, Email, Phone, Address, DateOfBirth) VALUES
    ('MEM-20250101-1001', 'John', 'Smith', 'john.smith@email.com', '+1-555-0101', '123 Main St, Springfield, IL 62701', '1990-05-15'),
    ('MEM-20250101-1002', 'Emma', 'Watson', 'emma.watson@email.com', '+1-555-0102', '456 Oak Ave, Portland, OR 97201', '1988-11-22'),
    ('MEM-20250101-1003', 'Michael', 'Brown', 'michael.brown@email.com', '+1-555-0103', '789 Pine Rd, Austin, TX 73301', '1995-03-08'),
    ('MEM-20250101-1004', 'Sarah', 'Wilson', 'sarah.wilson@email.com', '+1-555-0104', '321 Elm St, Denver, CO 80201', '2000-07-30'),
    ('MEM-20250101-1005', 'James', 'Taylor', 'james.taylor@email.com', '+1-555-0105', '654 Maple Dr, Boston, MA 02101', '1985-09-12');
END
GO

-- BorrowingTransactions
-- Requires AspNetUsers to be seeded by the application DataSeeder first.
GO

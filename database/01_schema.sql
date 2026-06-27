-- Library Management System Database Schema
CREATE DATABASE LibraryManagementDb;
GO

USE LibraryManagementDb;
GO


CREATE TABLE ActivityLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Action NVARCHAR(500) NOT NULL,
    EntityType NVARCHAR(100) NOT NULL,
    EntityId INT NULL,
    Details NVARCHAR(MAX) NULL,
    Timestamp DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IpAddress NVARCHAR(50) NULL,

    CONSTRAINT FK_ActivityLogs_AspNetUsers FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE INDEX IX_ActivityLogs_UserId ON ActivityLogs(UserId);
CREATE INDEX IX_ActivityLogs_Timestamp ON ActivityLogs(Timestamp);

CREATE TABLE Publishers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500) NULL,
    Phone NVARCHAR(50) NULL,
    Email NVARCHAR(255) NULL,
    Website NVARCHAR(255) NULL
);

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    ParentCategoryId INT NULL,

    CONSTRAINT FK_Categories_ParentCategory FOREIGN KEY (ParentCategoryId)
        REFERENCES Categories(Id) ON DELETE NO ACTION
);

CREATE INDEX IX_Categories_ParentCategoryId ON Categories(ParentCategoryId);

CREATE TABLE Authors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Biography NVARCHAR(MAX) NULL,
    BirthDate DATE NULL
);

CREATE INDEX IX_Authors_Name ON Authors(FirstName, LastName);

CREATE TABLE Books (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ISBN NVARCHAR(20) NOT NULL,
    Title NVARCHAR(500) NOT NULL,
    Edition NVARCHAR(100) NULL,
    PublicationYear INT NULL,
    Summary NVARCHAR(MAX) NULL,
    CoverImageUrl NVARCHAR(500) NULL,
    Status NVARCHAR(10) NOT NULL DEFAULT 'In', -- 'In' or 'Out'
    PageCount INT NULL,
    Language NVARCHAR(50) NOT NULL DEFAULT 'English',
    PublisherId INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT UQ_Books_ISBN UNIQUE (ISBN),
    CONSTRAINT FK_Books_Publishers FOREIGN KEY (PublisherId)
        REFERENCES Publishers(Id) ON DELETE SET NULL
);

CREATE INDEX IX_Books_Title ON Books(Title);
CREATE INDEX IX_Books_Status ON Books(Status);

CREATE TABLE BookAuthors (
    BookId INT NOT NULL,
    AuthorId INT NOT NULL,

    CONSTRAINT PK_BookAuthors PRIMARY KEY (BookId, AuthorId),
    CONSTRAINT FK_BookAuthors_Books FOREIGN KEY (BookId)
        REFERENCES Books(Id) ON DELETE CASCADE,
    CONSTRAINT FK_BookAuthors_Authors FOREIGN KEY (AuthorId)
        REFERENCES Authors(Id) ON DELETE CASCADE
);

CREATE TABLE BookCategories (
    BookId INT NOT NULL,
    CategoryId INT NOT NULL,

    CONSTRAINT PK_BookCategories PRIMARY KEY (BookId, CategoryId),
    CONSTRAINT FK_BookCategories_Books FOREIGN KEY (BookId)
        REFERENCES Books(Id) ON DELETE CASCADE,
    CONSTRAINT FK_BookCategories_Categories FOREIGN KEY (CategoryId)
        REFERENCES Categories(Id) ON DELETE CASCADE
);

CREATE TABLE Members (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MemberNumber NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(50) NULL,
    Address NVARCHAR(500) NULL,
    DateOfBirth DATE NULL,
    MembershipDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_Members_MemberNumber UNIQUE (MemberNumber),
    CONSTRAINT UQ_Members_Email UNIQUE (Email)
);

CREATE TABLE BorrowingTransactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BookId INT NOT NULL,
    MemberId INT NOT NULL,
    BorrowedById INT NOT NULL,
    BorrowDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DueDate DATETIME2 NOT NULL,
    ReturnDate DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Borrowed', -- Borrowed, Returned, Overdue
    Notes NVARCHAR(MAX) NULL,

    CONSTRAINT FK_BorrowingTransactions_Books FOREIGN KEY (BookId)
        REFERENCES Books(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_BorrowingTransactions_Members FOREIGN KEY (MemberId)
        REFERENCES Members(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_BorrowingTransactions_AspNetUsers FOREIGN KEY (BorrowedById)
        REFERENCES AspNetUsers(Id) ON DELETE NO ACTION
);

CREATE INDEX IX_BorrowingTransactions_BookId ON BorrowingTransactions(BookId);
CREATE INDEX IX_BorrowingTransactions_MemberId ON BorrowingTransactions(MemberId);
CREATE INDEX IX_BorrowingTransactions_Status ON BorrowingTransactions(Status);

CREATE TABLE RefreshTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Token NVARCHAR(512) NOT NULL,
    AppUserId INT NOT NULL,
    JwtId NVARCHAR(128) NOT NULL,
    IsUsed BIT NOT NULL DEFAULT 0,
    IsRevoked BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ExpiresAt DATETIME2 NOT NULL,

    CONSTRAINT FK_RefreshTokens_AspNetUsers FOREIGN KEY (AppUserId)
        REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX UQ_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IX_RefreshTokens_AppUserId ON RefreshTokens(AppUserId);
GO

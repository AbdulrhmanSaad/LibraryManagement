# Library Management System

A RESTful API for managing library operations — books, authors, members, borrowing transactions, system users, and activity audit logs. Built with **.NET 10**, **CQRS with MediatR**, and **Microsoft SQL Server** following **Clean Architecture** principles.

## Architecture

### Solution Structure (Clean Architecture)

```
LibraryManagement.sln
├── src/
│   ├── LibraryManagement.API            # Web API controllers, middleware, Program.cs
│   ├── LibraryManagement.Core           # Entities, interfaces, DTOs, CQRS commands/queries/handlers
│   └── LibraryManagement.Infrastructure  # DbContext, EF Core, repositories, mapping, JWT
├── tests/
│   ├── LibraryManagement.UnitTests                  
│   └── LibraryManagement.IntegrationTests
├── database/
│   ├── 01_schema.sql                    # Database schema
│   └── 02_ERD file
├── postman/
│   └── LibraryManagement.postman_collection.json  # Postman collection
└── README.md
```

### CQRS Structure (within Core)

Each feature group separates commands, queries, and their handlers into a consistent folder layout:

```
Commands/
├── Auth/              # Login, refresh token, revoke token
├── Authors/           # Create, update, delete author
├── Books/             # Create, update, delete book
├── Borrowing/         # Borrow, return book
├── Categories/        # Create, update, delete category
├── Members/           # Create, update, delete member
├── Publishers/        # Create, update, delete publisher
└── Users/             # Create, update, delete user

Queries/
├── ActivityLogs/      # Get recent activity logs
├── Authors/           # Get all, get by ID
├── Books/             # Get all, get by ID, search, by status
├── Borrowing/         # Get all, get by ID, by member, active
├── Categories/        # Get all, get by ID
├── Members/           # Get all, get by ID, get borrowings
├── Publishers/        # Get all, get by ID
└── Users/             # Get all, get by ID, get activity logs
```

Each feature folder contains:
- The command/query records (e.g., `AuthorCommands.cs`)
- FluentValidation validators (e.g., `AuthorCommandValidators.cs`)
- A `Handlers/` subfolder with the MediatR request handlers

### Design Decisions

| Decision | Rationale |
|---|---|
| **Clean Architecture** | Separation of concerns — API layer handles HTTP concerns, Core contains business logic and interfaces, Infrastructure handles data access. Enables testability and maintainability. |
| **CQRS with MediatR** | Commands and queries are separate, single-responsibility request handlers. Each handler is an isolated unit, making the codebase easier to reason about, test, and extend. |
| **Handlers/ subfolder convention** | Every feature group organises its request handlers in a `Handlers/` subfolder beneath the feature folder, keeping commands/queries and their validators at the feature root. |
| **FluentValidation** | Declarative validation rules co-located with commands/queries. Integrated via MediatR pipeline for automatic validation before handler execution. |
| **Repository Pattern** | Abstracts data access behind interfaces, making the service layer testable and DB-agnostic. |
| **AutoMapper** | Reduces boilerplate mapping between entities and DTOs. Prevents exposure of internal entity structure to API consumers. |
| **JWT Authentication** | Stateless, scalable authentication. Roles embedded in token claims for efficient authorization checks. |
| **SQL Server + EF Core** | Full relational database with ORM. EF Core provides LINQ queries, migrations, and relationship management. |

## Database Schema

10 tables covering:

- **Books** — Full bibliographic metadata (ISBN, title, edition, year, summary, language, status, cover image)
- **Authors** — Multi-author support via M:N junction table (`BookAuthors`)
- **Categories** — Hierarchical (self-referencing) classification
- **Publishers** — Publisher information
- **Members** — Borrower/patron information
- **Users** — System users with roles (Administrator, Librarian, Staff)
- **BorrowingTransactions** — Borrow/return lifecycle tracking
- **ActivityLogs** — Audit trail for user actions

## Role-Based Access Control

| Role | Permissions |
|---|---|
| **Administrator** | Full access — manage users, books, members, categories, view activity logs |
| **Librarian** | Manage books, members, categories, publishers, authors; process borrow/return |
| **Staff** | Read-only access to books, members, borrowing records; no create/update/delete |

## API Endpoints

### Authentication
| Method | Endpoint | Auth | Roles |
|---|---|---|---|
| POST | `/api/auth/login` | No | — |
| POST | `/api/auth/register` | Yes | Administrator |

### Books
| Method | Endpoint | Auth | Roles |
|---|---|---|---|
| GET | `/api/books` | Yes | All authenticated |
| GET | `/api/books/{id}` | Yes | All authenticated |
| GET | `/api/books/search?title=&author=&category=&language=&status=` | Yes | All authenticated |
| GET | `/api/books/status/{status}` | Yes | All authenticated |
| POST | `/api/books` | Yes | Administrator, Librarian |
| PUT | `/api/books/{id}` | Yes | Administrator, Librarian |
| DELETE | `/api/books/{id}` | Yes | Administrator |

### Members
| Method | Endpoint | Auth | Roles |
|---|---|---|---|
| GET | `/api/members` | Yes | All authenticated |
| GET | `/api/members/{id}` | Yes | All authenticated |
| GET | `/api/members/{id}/borrowings` | Yes | All authenticated |
| POST | `/api/members` | Yes | Administrator, Librarian |
| PUT | `/api/members/{id}` | Yes | Administrator, Librarian |
| DELETE | `/api/members/{id}` | Yes | Administrator |

### Borrowing
| Method | Endpoint | Auth | Roles |
|---|---|---|---|
| GET | `/api/borrowing` | Yes | All authenticated |
| GET | `/api/borrowing/{id}` | Yes | All authenticated |
| GET | `/api/borrowing/active` | Yes | All authenticated |
| GET | `/api/borrowing/member/{memberId}` | Yes | All authenticated |
| POST | `/api/borrowing/borrow` | Yes | Administrator, Librarian |
| POST | `/api/borrowing/return` | Yes | Administrator, Librarian |

### Users (Admin only)
| Method | Endpoint | Auth | Roles |
|---|---|---|---|
| GET | `/api/users` | Yes | Administrator |
| GET | `/api/users/{id}` | Yes | Administrator |
| GET | `/api/users/{id}/activity` | Yes | Administrator |
| POST | `/api/users` | Yes | Administrator |
| PUT | `/api/users/{id}` | Yes | Administrator |
| DELETE | `/api/users/{id}` | Yes | Administrator |

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (local or remote)

### Setup

```bash
# 1. Clone and restore
git clone <repo>
cd LibraryManagement
dotnet restore

# 2. Create database
sqlcmd -S localhost -i database/01_schema.sql
sqlcmd -S localhost -d LibraryManagementDb -i database/02_seed_data.sql

# 3. Update connection string in appsettings.json if needed
#    Default: Server=localhost;Database=LibraryManagementDb;Trusted_Connection=True;

# 4. Run
dotnet run --project src/LibraryManagement.API
```

### Default Login Credentials

| Username | Password | Role |
|---|---|---|
| `admin` | `Password123` | Administrator |
| `librarian1` | `Password123` | Librarian |
| `staff1` | `Password123` | Staff |
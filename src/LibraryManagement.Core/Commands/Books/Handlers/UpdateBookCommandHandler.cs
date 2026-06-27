using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Books;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Books.Handlers;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand>
{
    private readonly IBookRepository _bookRepository;
    private readonly IGenericRepository<Author> _authorRepository;
    private readonly IGenericRepository<Category> _categoryRepository;

    public UpdateBookCommandHandler(IBookRepository bookRepository, IGenericRepository<Author> authorRepository, IGenericRepository<Category> categoryRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Book;

        var book = await _bookRepository.GetBookWithDetailsAsync(request.Id)
            ?? throw new KeyNotFoundException($"Book with Id '{request.Id}' was not found.");

        // Update scalar properties
        book.ISBN = dto.ISBN ?? book.ISBN;
        book.Title = dto.Title ?? book.Title;
        book.Edition = dto.Edition ?? book.Edition;
        book.PublicationYear = dto.PublicationYear ?? book.PublicationYear;
        book.Summary = dto.Summary ?? book.Summary;
        book.CoverImageUrl = dto.CoverImageUrl ?? book.CoverImageUrl;
        book.PageCount = dto.PageCount ?? book.PageCount;
        book.Language = dto.Language ?? book.Language;
        book.PublisherId = dto.PublisherId ?? book.PublisherId;

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            if (!Enum.TryParse<BookStatus>(dto.Status, true, out var status))
                throw new ArgumentException("Invalid book status.");

            book.Status = status;
        }

        // Update Authors
        if (dto.AuthorIds is not null)
        {
            var authors = (await _authorRepository.FindAsync(a => dto.AuthorIds.Contains(a.Id))).ToList();

            if (authors.Count != dto.AuthorIds.Count)
                throw new ArgumentException("One or more AuthorIds are invalid.");

            book.Authors.Clear();

            foreach (var author in authors)
                book.Authors.Add(author);
        }

        // Update Categories
        if (dto.CategoryIds is not null)
        {
            var categories = (await _categoryRepository.FindAsync(c => dto.CategoryIds.Contains(c.Id))).ToList();

            if (categories.Count != dto.CategoryIds.Count)
                throw new ArgumentException("One or more CategoryIds are invalid.");

            book.Categories.Clear();

            foreach (var category in categories)
                book.Categories.Add(category);
        }

        book.UpdatedAt = DateTime.UtcNow;

        await _bookRepository.UpdateAsync(book);
    }
}

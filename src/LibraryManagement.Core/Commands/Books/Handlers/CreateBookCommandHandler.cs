using AutoMapper;
using MediatR;
using LibraryManagement.Core.Commands.Books;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Core.Commands.Books.Handlers;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly IBookRepository _bookRepository;
    private readonly IGenericRepository<Author> _authorRepository;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;

    public CreateBookCommandHandler(IBookRepository bookRepository, IGenericRepository<Author> authorRepository, IGenericRepository<Category> categoryRepository, IMapper mapper)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var existing = await _bookRepository.FindAsync(b => b.ISBN == request.Book.ISBN);
        if (existing.Any())
            throw new InvalidOperationException("A book with this ISBN already exists");

        var book = _mapper.Map<Book>(request.Book);
        book.Status = BookStatus.In;
        book.CreatedAt = DateTime.UtcNow;

        if (request.Book.AuthorIds.Count != 0)
        {
            var authors = await _authorRepository.FindAsync(a => request.Book.AuthorIds.Contains(a.Id));
            foreach (var author in authors) book.Authors.Add(author);
        }

        if (request.Book.CategoryIds.Count != 0)
        {
            var categories = await _categoryRepository.FindAsync(c => request.Book.CategoryIds.Contains(c.Id));
            foreach (var category in categories) book.Categories.Add(category);
        }

        await _bookRepository.AddAsync(book);
        return _mapper.Map<BookDto>(await _bookRepository.GetBookWithDetailsAsync(book.Id));
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Commands.Books;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Queries.Books;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        => Ok(await _mediator.Send(new GetAllBooksQuery()));

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(int id)
    {
        var book = await _mediator.Send(new GetBookByIdQuery(id));
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BookDto>>> Search(
        [FromQuery] string? title, [FromQuery] string? author, [FromQuery] string? category,
        [FromQuery] string? language, [FromQuery] string? status)
        => Ok(await _mediator.Send(new SearchBooksQuery(title, author, category, language, status)));

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetByStatus(string status)
    {
        if (!Enum.TryParse<BookStatus>(status, true, out var parsedStatus))
            return BadRequest(new { message = "Status must be 'In' or 'Out'" });
        return Ok(await _mediator.Send(new GetBooksByStatusQuery(parsedStatus)));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
    {
        try
        {
            var book = await _mediator.Send(new CreateBookCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
    {
        try
        {
            await _mediator.Send(new UpdateBookCommand(id, dto));
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _mediator.Send(new DeleteBookCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}

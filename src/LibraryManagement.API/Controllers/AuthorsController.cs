using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Commands.Authors;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Queries.Authors;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthorsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll()
        => Ok(await _mediator.Send(new GetAllAuthorsQuery()));

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetById(int id)
    {
        var author = await _mediator.Send(new GetAuthorByIdQuery(id));
        if (author == null) return NotFound();
        return Ok(author);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorDto dto)
    {
        var author = await _mediator.Send(new CreateAuthorCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorDto dto)
    {
        try
        {
            await _mediator.Send(new UpdateAuthorCommand(id, dto));
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
            await _mediator.Send(new DeleteAuthorCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}

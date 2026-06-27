using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Commands.Publishers;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Queries.Publishers;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PublishersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublishersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PublisherDto>>> GetAll()
        => Ok(await _mediator.Send(new GetAllPublishersQuery()));

    [HttpGet("{id}")]
    public async Task<ActionResult<PublisherDto>> GetById(int id)
    {
        var publisher = await _mediator.Send(new GetPublisherByIdQuery(id));
        if (publisher == null) return NotFound();
        return Ok(publisher);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<ActionResult<PublisherDto>> Create([FromBody] CreatePublisherDto dto)
    {
        var publisher = await _mediator.Send(new CreatePublisherCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = publisher.Id }, publisher);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePublisherDto dto)
    {
        try
        {
            await _mediator.Send(new UpdatePublisherCommand(id, dto));
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
            await _mediator.Send(new DeletePublisherCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}

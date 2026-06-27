using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Commands.Members;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Queries.Members;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Administrator,Librarian,Staff")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll()
        => Ok(await _mediator.Send(new GetAllMembersQuery()));

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator,Librarian,Staff")]
    public async Task<ActionResult<MemberDto>> GetById(int id)
    {
        var member = await _mediator.Send(new GetMemberByIdQuery(id));
        if (member == null) return NotFound();
        return Ok(member);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberDto dto)
    {
        try
        {
            var member = await _mediator.Send(new CreateMemberCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMemberDto dto)
    {
        try
        {
            await _mediator.Send(new UpdateMemberCommand(id, dto));
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
            await _mediator.Send(new DeleteMemberCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("{id}/borrowings")]
    [Authorize(Roles = "Administrator,Librarian,Staff")]
    public async Task<ActionResult<IEnumerable<BorrowingTransactionDto>>> GetBorrowings(int id)
        => Ok(await _mediator.Send(new GetMemberBorrowingsQuery(id)));
}

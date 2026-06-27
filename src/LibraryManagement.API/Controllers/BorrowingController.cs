using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Commands.Borrowing;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Queries.Borrowing;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BorrowingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BorrowingController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Administrator,Librarian,Staff")]
    public async Task<ActionResult<IEnumerable<BorrowingTransactionDto>>> GetAll()
        => Ok(await _mediator.Send(new GetAllBorrowingsQuery()));

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator,Librarian,Staff")]
    public async Task<ActionResult<BorrowingTransactionDto>> GetById(int id)
    {
        var t = await _mediator.Send(new GetBorrowingByIdQuery(id));
        if (t == null) return NotFound();
        return Ok(t);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Administrator,Librarian,Staff")]
    public async Task<ActionResult<IEnumerable<BorrowingTransactionDto>>> GetActive()
        => Ok(await _mediator.Send(new GetActiveBorrowingsQuery()));

    [HttpPost("borrow")]
    [Authorize(Roles = "Administrator,Librarian")]
    public async Task<ActionResult<BorrowingTransactionDto>> Borrow([FromBody] CreateBorrowingDto dto)
    {
        try
        {
            var t = await _mediator.Send(new BorrowBookCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = t.Id }, t);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("return")]
    [Authorize(Roles = "Administrator,Librarian,Staff")]
    public async Task<ActionResult<BorrowingTransactionDto>> Return([FromBody] ReturnBookDto dto)
    {
        try
        {
            return Ok(await _mediator.Send(new ReturnBookCommand(dto)));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("member/{memberId}")]
    [Authorize(Roles = "Administrator,Librarian,Staff")]
    public async Task<ActionResult<IEnumerable<BorrowingTransactionDto>>> GetByMember(int memberId)
        => Ok(await _mediator.Send(new GetBorrowingsByMemberQuery(memberId)));
}

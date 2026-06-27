using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Queries.ActivityLogs;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class ActivityLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ActivityLogsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityLogDto>>> GetRecent([FromQuery] int count = 50)
        => Ok(await _mediator.Send(new GetRecentActivityLogsQuery(count)));
}

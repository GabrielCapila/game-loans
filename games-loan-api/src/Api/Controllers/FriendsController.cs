using GamesLoan.Api.DTOs.Friends;
using GamesLoan.Application.Friends.Command.CreateFriend;
using GamesLoan.Application.Friends.Models;
using GamesLoan.Application.Friends.Queries.DeleteFriend;
using GamesLoan.Application.Friends.Queries.GetFriendById;
using GamesLoan.Application.Friends.Queries.ListFriends;
using GamesLoan.Application.Friends.Queries.UpdateFriend;
using GamesLoan.Application.Friends.Update;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesLoan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FriendsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FriendsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/friends
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFriendRequest request)
    {
        var id = await _mediator.Send(
            new CreateFriendCommand(request.Name, request.Email, request.Phone)
        );

        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    // GET api/friends
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FriendDto>>> GetAll()
    {
        var friends = await _mediator.Send(new ListFriendsQuery());
        return Ok(friends);
    }

    // GET api/friends/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FriendDto>> GetById(int id)
    {
        var friend = await _mediator.Send(new GetFriendByIdQuery(id));

        if (friend is null)
            return NotFound();

        return Ok(friend);
    }

    // PUT api/friends/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFriendRequest request)
    {
        var success = await _mediator.Send(
            new UpdateFriendCommand(id, request.Name, request.Email, request.Phone)
        );

        if (!success)
            return NotFound();

        return NoContent();
    }

    // DELETE api/friends/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mediator.Send(new DeleteFriendCommand(id));

        if (!success)
            return NotFound();

        return NoContent();
    }
}
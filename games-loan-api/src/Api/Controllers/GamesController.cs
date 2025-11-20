using GamesLoan.Api.DTOs.Games;
using GamesLoan.Application.Games.Commands.CreateGame;
using GamesLoan.Application.Games.Models;
using GamesLoan.Application.Games.Queries.ListGames;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesLoan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly IMediator _mediator;

    public GamesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GameDto>>> GetAll()
    {
        var games = await _mediator.Send(new ListGamesQuery());
        return Ok(games);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGameRequest request)
    {
        var id = await _mediator.Send(new CreateGameCommand(
            request.Name,
            request.Publishers,
            request.Genre,
            request.ExternalSourceId
        ));

        return CreatedAtAction(nameof(GetAll), new { id }, null);
    }
}
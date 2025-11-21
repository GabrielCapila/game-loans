using GamesLoan.Api.DTOs.Games;
using GamesLoan.Application.Games.Commands.CreateGame;
using GamesLoan.Application.Games.Commands.UpdateGame;
using GamesLoan.Application.Games.Commands.DeleteGame;
using GamesLoan.Application.Games.Models;
using GamesLoan.Application.Games.Queries.ListGames;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesLoan.Api.Controllers;

/// <summary>
/// Operações relacionadas a jogos (Game).
/// </summary>
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

    /// <summary>
    /// Lista todos os jogos.
    /// </summary>
    /// <returns>Lista de jogos com status de empréstimo.</returns>
    /// <response code="200">Lista retornada.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GameDto>>> GetAll()
    {
        var games = await _mediator.Send(new ListGamesQuery());
        return Ok(games);
    }

    /// <summary>
    /// Cria um novo jogo.
    /// </summary>
    /// <param name="request">Dados do jogo.</param>
    /// <returns>Location do recurso criado.</returns>
    /// <response code="201">Jogo criado com sucesso.</response>
    /// <response code="409">Dados inválidos.</response>
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

    /// <summary>
    /// Atualiza dados de um jogo (não permitido se estiver emprestado).
    /// </summary>
    /// <param name="id">Id do jogo.</param>
    /// <param name="request">Novos dados.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Atualizado com sucesso.</response>
    /// <response code="404">Jogo não encontrado.</response>
    /// <response code="409">Jogo emprestado não pode ser alterado.</response>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGameRequest request)
    {
        var result = await _mediator.Send(new UpdateGameCommand(
            Id: id,
            Name: request.Name,
            Publishers: request.Publishers,
            Genre: request.Genre
        ));

        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Exclui um jogo (não permitido se estiver emprestado).
    /// </summary>
    /// <param name="id">Id do jogo.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Removido com sucesso.</response>
    /// <response code="404">Jogo não encontrado.</response>
    /// <response code="409">Jogo emprestado não pode ser removido.</response>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mediator.Send(new DeleteGameCommand(id));
        if (!success)
            return NotFound();
        return NoContent();
    }
}
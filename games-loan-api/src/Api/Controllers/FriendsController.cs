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

/// <summary>
/// Operações relacionadas a amigos (Friend).
/// </summary>
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

    /// <summary>
    /// Cria um novo amigo.
    /// </summary>
    /// <param name="request">Dados do amigo (nome, email, telefone).</param>
    /// <returns>Location com o id do recurso criado.</returns>
    /// <response code="201">Amigo criado com sucesso.</response>
    /// <response code="409">E-mail já existente ou dados inválidos.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFriendRequest request)
    {
        var id = await _mediator.Send(
            new CreateFriendCommand(request.Name, request.Email, request.Phone)
        );

        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Retorna todos os amigos.
    /// </summary>
    /// <returns>Lista de amigos.</returns>
    /// <response code="200">Lista retornada.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FriendDto>>> GetAll()
    {
        var friends = await _mediator.Send(new ListFriendsQuery());
        return Ok(friends);
    }

    /// <summary>
    /// Retorna um amigo pelo id.
    /// </summary>
    /// <param name="id">Id do amigo.</param>
    /// <returns>Dados do amigo.</returns>
    /// <response code="200">Amigo encontrado.</response>
    /// <response code="404">Amigo não encontrado.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FriendDto>> GetById(int id)
    {
        var friend = await _mediator.Send(new GetFriendByIdQuery(id));

        if (friend is null)
            return NotFound();

        return Ok(friend);
    }

    /// <summary>
    /// Atualiza dados de um amigo.
    /// </summary>
    /// <param name="id">Id do amigo.</param>
    /// <param name="request">Novos dados.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Atualizado com sucesso.</response>
    /// <response code="404">Amigo não encontrado.</response>
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

    /// <summary>
    /// Remove (soft-delete) um amigo.
    /// </summary>
    /// <param name="id">Id do amigo.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Removido com sucesso.</response>
    /// <response code="404">Amigo não encontrado.</response>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mediator.Send(new DeleteFriendCommand(id));

        if (!success)
            return NotFound();

        return NoContent();
    }
}
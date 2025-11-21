using GamesLoan.Api.DTOs.Loans;
using GamesLoan.Application.Loans.Commands.CreateLoan;
using GamesLoan.Application.Loans.Commands.RegisterReturn;
using GamesLoan.Application.Loans.Commands.UpdateLoan;
using GamesLoan.Application.Loans.Models;
using GamesLoan.Application.Loans.Queries.ListActiveLoans;
using GamesLoan.Application.Loans.Queries.ListLoans;
using GamesLoan.Application.Loans.Queries.ListLoansByFriend;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesLoan.Api.Controllers;

/// <summary>
/// Operações relacionadas a empréstimos (Loan).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cria um novo empréstimo de jogo para um amigo.
    /// </summary>
    /// <param name="request">Dados do empréstimo (FriendId, GameId, ExpectedReturnDate).</param>
    /// <returns>Informações do empréstimo criado.</returns>
    /// <response code="201">Empréstimo criado.</response>
    /// <response code="404">Amigo ou Jogo não encontrados.</response>
    /// <response code="409">Regra de domínio violada (ex: jogo já emprestado).</response>
    [HttpPost]
    public async Task<ActionResult<LoanCreatedDto>> Create([FromBody] CreateLoanRequest request)
    {
        var result = await _mediator.Send(new CreateLoanCommand(
            request.FriendId,
            request.GameId,
            request.ExpectedReturnDate
        ));

        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    /// <summary>
    /// Registra a devolução de um empréstimo.
    /// </summary>
    /// <param name="loanId">Id do empréstimo.</param>
    /// <returns>Informações da devolução.</returns>
    /// <response code="200">Devolução registrada.</response>
    /// <response code="404">Empréstimo/Jogo/Amigo não encontrados.</response>
    /// <response code="409">Regra de domínio violada (ex: empréstimo já devolvido).</response>
    [HttpPost("{loanId:int}/return")]
    public async Task<ActionResult<LoanReturnResultDto>> Return(int loanId)
    {
        var result = await _mediator.Send(new RegisterReturnCommand(loanId));
        return Ok(result);
    }

    /// <summary>
    /// Lista todos os empréstimos (opcional apenas ativos).
    /// </summary>
    /// <param name="onlyActive">Se true, retorna apenas empréstimos abertos.</param>
    /// <returns>Lista de empréstimos.</returns>
    /// <response code="200">Lista retornada.</response>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LoanDetailsDto>>> GetAll([FromQuery] bool onlyActive = false)
    {
        var loans = await _mediator.Send(new ListLoansQuery(onlyActive));
        return Ok(loans);
    }

    /// <summary>
    /// Lista empréstimos de um amigo específico.
    /// </summary>
    /// <param name="friendId">Id do amigo.</param>
    /// <returns>Lista de empréstimos do amigo.</returns>
    /// <response code="200">Lista retornada.</response>
    /// <response code="404">Amigo não encontrado.</response>
    [HttpGet("by-friend/{friendId:int}")]
    public async Task<ActionResult<IReadOnlyList<LoanDetailsDto>>> GetByFriend(int friendId)
    {
        var loans = await _mediator.Send(new ListLoansByFriendQuery(friendId));
        return Ok(loans);
    }

    /// <summary>
    /// Atualiza a data de devolução prevista de um empréstimo.
    /// </summary>
    /// <param name="id">Id do empréstimo.</param>
    /// <param name="request">Nova data prevista.</param>
    /// <returns>Dados atualizados.</returns>
    /// <response code="200">Atualização realizada.</response>
    /// <response code="404">Empréstimo não encontrado.</response>
    /// <response code="409">Regra de domínio violada.</response>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
    int id,
    [FromBody] UpdateLoanRequest request)
    {
        var result = await _mediator.Send(new UpdateLoanCommand(
            LoanId: id,
            ExpectedReturnDate: request.ExpectedReturnDate
        ));

        return Ok(result);
    }

}

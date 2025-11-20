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

    [HttpPost("{loanId:int}/return")]
    public async Task<ActionResult<LoanReturnResultDto>> Return(int loanId)
    {
        var result = await _mediator.Send(new RegisterReturnCommand(loanId));
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LoanDetailsDto>>> GetAll([FromQuery] bool onlyActive = false)
    {
        var loans = await _mediator.Send(new ListLoansQuery(onlyActive));
        return Ok(loans);
    }

    [HttpGet("by-friend/{friendId:int}")]
    public async Task<ActionResult<IReadOnlyList<LoanDetailsDto>>> GetByFriend(int friendId)
    {
        var loans = await _mediator.Send(new ListLoansByFriendQuery(friendId));
        return Ok(loans);
    }

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

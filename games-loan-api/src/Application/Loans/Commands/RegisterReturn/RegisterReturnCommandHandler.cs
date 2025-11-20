using GamesLoan.Application.Exceptions;
using GamesLoan.Application.Loans.Models;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using GamesLoan.Domain.Types;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Commands.RegisterReturn;
public sealed class RegisterReturnCommandHandler
    : IRequestHandler<RegisterReturnCommand, LoanReturnResultDto>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IFriendRepository _friendRepository;

    public RegisterReturnCommandHandler(
        ILoanRepository loanRepository,
        IGameRepository gameRepository,
        IFriendRepository friendRepository)
    {
        _loanRepository = loanRepository;
        _gameRepository = gameRepository;
        _friendRepository = friendRepository;
    }

    public async Task<LoanReturnResultDto> Handle(RegisterReturnCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken)
            ?? throw new NotFoundException($"Loan with id {request.LoanId} was not found.");

        if (loan.Status != LoanStatus.Open)
            throw new DomainException("Only open loans can be returned.");

        loan.RegisterReturn(DateTime.UtcNow);

        var game = await _gameRepository.GetByIdAsync(loan.GameId, cancellationToken)
            ?? throw new NotFoundException($"Game with id {loan.GameId} was not found.");

        var friend = await _friendRepository.GetByIdAsync(loan.FriendId, cancellationToken)
            ?? throw new NotFoundException($"Friend with id {loan.FriendId} was not found.");

        game.MarkAsReturned();

        await _loanRepository.UpdateAsync(loan, cancellationToken);
        await _gameRepository.UpdateAsync(game, cancellationToken);

        return new LoanReturnResultDto
        {
            LoanId = loan.Id,
            FriendId = friend.Id,
            FriendName = friend.Name,
            GameId = game.Id,
            GameTitle = game.Name,
            Message = $"The game '{game.Name}' has been returned by '{friend.Name}'."
        };
    }
}

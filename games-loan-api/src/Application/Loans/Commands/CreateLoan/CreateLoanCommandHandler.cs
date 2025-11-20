using GamesLoan.Application.Exceptions;
using GamesLoan.Application.Loans.Models;
using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Commands.CreateLoan;

public sealed class CreateLoanCommandHandler
    : IRequestHandler<CreateLoanCommand, LoanCreatedDto>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IFriendRepository _friendRepository;
    private readonly IGameRepository _gameRepository;

    public CreateLoanCommandHandler(
        ILoanRepository loanRepository,
        IFriendRepository friendRepository,
        IGameRepository gameRepository)
    {
        _loanRepository = loanRepository;
        _friendRepository = friendRepository;
        _gameRepository = gameRepository;
    }

    public async Task<LoanCreatedDto> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        var friend = await _friendRepository.GetByIdAsync(request.FriendId, cancellationToken);
        if (friend is null)
            throw new NotFoundException($"Friend with id {request.FriendId} was not found.");

        var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
        if (game is null)
            throw new NotFoundException($"Game with id {request.GameId} was not found.");

        if (game.IsLoaned)
            throw new DomainException($"The game '{game.Name}' is already loaned.");

        if (request.ExpectedReturnDate == default)
            throw new DomainException("Expected return date is required.");

        if (request.ExpectedReturnDate.Date < DateTime.UtcNow.Date)
            throw new DomainException("Expected return date cannot be in the past.");

        var loan = new Loan(
            gameId: request.GameId,
            friendId: request.FriendId,
            loanDate: DateTime.UtcNow,
            expectedReturnDate: request.ExpectedReturnDate
        );

        game.MarkAsLoaned();

        await _loanRepository.AddAsync(loan, cancellationToken);
        await _gameRepository.UpdateAsync(game, cancellationToken);

        return new LoanCreatedDto
        {
            LoanId = loan.Id,
            FriendId = friend.Id,
            GameId = game.Id,
            Message = $"You just loaned '{game.Name}' to '{friend.Name}'."
        };
    }
}

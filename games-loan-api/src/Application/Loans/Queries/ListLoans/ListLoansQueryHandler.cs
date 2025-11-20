using GamesLoan.Application.Loans.Models;
using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Queries.ListLoans;
public sealed class ListLoansQueryHandler
    : IRequestHandler<ListLoansQuery, IReadOnlyList<LoanDetailsDto>>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IFriendRepository _friendRepository;
    private readonly IGameRepository _gameRepository;

    public ListLoansQueryHandler(
        ILoanRepository loanRepository,
        IFriendRepository friendRepository,
        IGameRepository gameRepository)
    {
        _loanRepository = loanRepository;
        _friendRepository = friendRepository;
        _gameRepository = gameRepository;
    }

    public async Task<IReadOnlyList<LoanDetailsDto>> Handle(
        ListLoansQuery request,
        CancellationToken cancellationToken)
    {
        var loans = request.OnlyActive
            ? await _loanRepository.GetOpenLoansAsync(cancellationToken)
            : await _loanRepository.GetAllAsync(cancellationToken);

        if (loans.Count == 0)
            return Array.Empty<LoanDetailsDto>();

        var friendIds = loans.Select(l => l.FriendId).Distinct().ToList();
        var gameIds = loans.Select(l => l.GameId).Distinct().ToList();

        var friends = new Dictionary<int, Friend>();
        foreach (var id in friendIds)
        {
            var f = await _friendRepository.GetByIdAsync(id, cancellationToken);
            if (f != null)
                friends[id] = f;
        }

        var games = new Dictionary<int, Game>();
        foreach (var id in gameIds)
        {
            var g = await _gameRepository.GetByIdAsync(id, cancellationToken);
            if (g != null)
                games[id] = g;
        }

        return loans.Select(l =>
        {
            friends.TryGetValue(l.FriendId, out var friend);
            games.TryGetValue(l.GameId, out var game);

            return new LoanDetailsDto
            {
                Id = l.Id,
                FriendId = l.FriendId,
                FriendName = friend?.Name ?? "(unknown friend)",
                GameId = l.GameId,
                GameTitle = game?.Name ?? "(unknown game)",
                LoanDate = l.LoanDate,
                ExpectedReturnDate = l.ExpectedReturnDate,
                ReturnDate = l.ActualReturnDate,
                Status = l.Status.ToString()
            };
        }).ToList();
    }
}
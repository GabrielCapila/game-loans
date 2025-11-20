using GamesLoan.Application.Exceptions;
using GamesLoan.Application.Loans.Models;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Queries.ListLoansByFriend;
public sealed class ListLoansByFriendQueryHandler
    : IRequestHandler<ListLoansByFriendQuery, IReadOnlyList<LoanDetailsDto>>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IFriendRepository _friendRepository;
    private readonly IGameRepository _gameRepository;

    public ListLoansByFriendQueryHandler(
        ILoanRepository loanRepository,
        IFriendRepository friendRepository,
        IGameRepository gameRepository)
    {
        _loanRepository = loanRepository;
        _friendRepository = friendRepository;
        _gameRepository = gameRepository;
    }

    public async Task<IReadOnlyList<LoanDetailsDto>> Handle(
        ListLoansByFriendQuery request,
        CancellationToken cancellationToken)
    {
        var friend = await _friendRepository.GetByIdAsync(request.FriendId, cancellationToken)
            ?? throw new NotFoundException($"Friend with id {request.FriendId} was not found.");

        var loans = await _loanRepository.GetLoansByFriendAsync(request.FriendId, cancellationToken);

        if (loans.Count == 0)
            return Array.Empty<LoanDetailsDto>();

        var gameIds = loans.Select(l => l.GameId).Distinct().ToList();
        var gamesTasks = gameIds.Select(id => _gameRepository.GetByIdAsync(id, cancellationToken));
        var games = (await Task.WhenAll(gamesTasks))
            .Where(g => g is not null)
            .ToDictionary(g => g!.Id, g => g!);

        return loans.Select(l =>
        {
            games.TryGetValue(l.GameId, out var game);

            return new LoanDetailsDto
            {
                Id = l.Id,
                FriendId = friend.Id,
                FriendName = friend.Name,
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
using GamesLoan.Application.Games.Models;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Games.Queries.ListGames;
public sealed class ListGamesQueryHandler
    : IRequestHandler<ListGamesQuery, IReadOnlyList<GameDto>>
{
    private readonly IGameRepository _gameRepository;

    public ListGamesQueryHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<IReadOnlyList<GameDto>> Handle(
        ListGamesQuery request,
        CancellationToken cancellationToken)
    {
        var games = await _gameRepository.GetAllAsync(cancellationToken);

        return games
            .Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Publishers = g.Publishers,
                Genre = g.Genre,
                ExternalSourceId = g.ExternalSourceId,
                IsLoaned = g.IsLoaned,
                CreatedAt = g.CreatedAt
            })
            .ToList();
    }
}

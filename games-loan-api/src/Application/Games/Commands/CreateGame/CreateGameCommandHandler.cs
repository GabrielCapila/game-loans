using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Games.Commands.CreateGame;
public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, int>
{
    private readonly IGameRepository _repo;

    public CreateGameCommandHandler(IGameRepository repo)
    {
        _repo = repo;
    }

    public async Task<int> Handle(CreateGameCommand req, CancellationToken ct)
    {
        var publisher = req.Publishers.FirstOrDefault();
        var genre = req.Genre.FirstOrDefault();

        var game = new Game(
            name: req.Name,
            publishers: new List<string> { publisher },
            genre: new List<string> { genre },
            externalSourceId: req.ExternalSourceId ?? ""
        );

        await _repo.AddAsync(game, ct);

        return game.Id;
    }
}
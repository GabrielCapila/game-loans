using GamesLoan.Application.Exceptions;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GamesLoan.Application.Games.Commands.UpdateGame;
public sealed class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand, bool>
{
    private readonly IGameRepository _gameRepository;

    public UpdateGameCommandHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<bool> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.Id, cancellationToken);
        if (game is null)
            throw new NotFoundException($"Game with id {request.Id} was not found.");

        if (game.IsLoaned)
            throw new DomainException("Cannot update a game while it is loaned.");

        game.SetName(request.Name);
        game.SetPublishers(request.Publishers);
        game.SetGenre(request.Genre);

        await _gameRepository.UpdateAsync(game, cancellationToken);
        return true;
    }
}
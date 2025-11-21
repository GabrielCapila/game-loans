using GamesLoan.Application.Exceptions;
using GamesLoan.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GamesLoan.Application.Games.Commands.DeleteGame;
public sealed class DeleteGameCommandHandler : IRequestHandler<DeleteGameCommand, bool>
{
    private readonly IGameRepository _gameRepository;

    public DeleteGameCommandHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(request.Id, cancellationToken);
        if (game is null)
            return false;

        if (game.IsLoaned)
            return false;

        await _gameRepository.DeleteAsync(game, cancellationToken);
        return true;
    }
}
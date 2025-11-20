using FluentAssertions;
using GamesLoan.Application.Games.Commands.CreateGame;
using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using Moq;

namespace GamesLoan.Tests.Games;

public class CreateGameCommandHandlerTests
{
    private readonly Mock<IGameRepository> _gameRepo = new();

    private CreateGameCommandHandler CreateHandler()
        => new(_gameRepo.Object);

    [Fact]
    public async Task Should_create_game_successfully_using_first_items()
    {
        var handler = CreateHandler();

        _gameRepo.Setup(r => r.AddAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CreateGameCommand(
            Name: "God of War",
            Publishers: new() { "Sony", "Santa Monica" },
            Genre: new() { "Action", "Adventure" },
            ExternalSourceId: null
        );

        var id = await handler.Handle(command, CancellationToken.None);

        _gameRepo.Verify(r => r.AddAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

using FluentAssertions;
using GamesLoan.Application.Exceptions;
using GamesLoan.Application.Loans.Commands.CreateLoan;
using GamesLoan.Application.Loans.Models;
using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using Moq;

namespace GamesLoan.Tests.Loans;

public class CreateLoanCommandHandlerTests
{
    private readonly Mock<ILoanRepository> _loanRepo = new();
    private readonly Mock<IFriendRepository> _friendRepo = new();
    private readonly Mock<IGameRepository> _gameRepo = new();

    private CreateLoanCommandHandler CreateHandler()
        => new(_loanRepo.Object, _friendRepo.Object, _gameRepo.Object);


    [Fact]
    public async Task Should_throw_NotFound_when_friend_does_not_exist()
    {
        var handler = CreateHandler();

        _friendRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Friend?)null);

        var command = new CreateLoanCommand(
            FriendId: 1,
            GameId: 1,
            ExpectedReturnDate: DateTime.UtcNow.AddDays(7)
        );

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Friend with id 1 was not found*");
    }

    [Fact]
    public async Task Should_throw_DomainException_when_game_already_loaned()
    {
        var handler = CreateHandler();

        var friend = new Friend("Gabriel", "gabriel@test.com", "31999999999");
        var game = new Game("FIFA 25", new() { "EA" }, new() { "Sports" }, "ext-1");
        game.MarkAsLoaned();

        _friendRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(friend);

        _gameRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);

        var command = new CreateLoanCommand(
            FriendId: 1,
            GameId: 1,
            ExpectedReturnDate: DateTime.UtcNow.AddDays(7)
        );

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("The game*already loaned*");
    }

    [Fact]
    public async Task Should_create_loan_successfully_and_return_message()
    {
        var handler = CreateHandler();

        var friend = new Friend("Gabriel", "gabriel@test.com", "31999999999");
        var game = new Game("FIFA 25", new() { "EA" }, new() { "Sports" }, "ext-1");

        _friendRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(friend);

        _gameRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(game);

        _loanRepo.Setup(r => r.AddAsync(It.IsAny<Loan>(), It.IsAny<CancellationToken>()))
            .Callback<Loan, CancellationToken>((loan, _) => loan.GetType()) // só pra não ficar vazio
            .Returns(Task.CompletedTask);

        var command = new CreateLoanCommand(
            FriendId: 1,
            GameId: 1,
            ExpectedReturnDate: DateTime.UtcNow.AddDays(7)
        );

        // act
        var result = await handler.Handle(command, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.Should().BeOfType<LoanCreatedDto>();
        result.Message.Should().Contain("You just loaned");
        result.Message.Should().Contain("FIFA 25");
        result.Message.Should().Contain("Gabriel");

        _loanRepo.Verify(r => r.AddAsync(It.IsAny<Loan>(), It.IsAny<CancellationToken>()), Times.Once);
        _gameRepo.Verify(r => r.UpdateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

using FluentAssertions;
using GamesLoan.Application.Exceptions;
using GamesLoan.Application.Loans.Commands.RegisterReturn;
using GamesLoan.Application.Loans.Models;
using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using Moq;

namespace GamesLoan.Tests.Loans;

public class RegisterReturnCommandHandlerTests
{
    private readonly Mock<ILoanRepository> _loanRepo = new();
    private readonly Mock<IGameRepository> _gameRepo = new();
    private readonly Mock<IFriendRepository> _friendRepo = new();

    private RegisterReturnCommandHandler CreateHandler()
        => new(_loanRepo.Object, _gameRepo.Object, _friendRepo.Object);

    [Fact]
    public async Task Should_throw_NotFound_when_loan_does_not_exist()
    {
        // arrange
        var handler = CreateHandler();

        _loanRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Loan?)null);

        var command = new RegisterReturnCommand(1);

        // act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Loan with id 1 was not found*");
    }

    [Fact]
    public async Task Should_throw_DomainException_when_loan_is_not_open()
    {
        var handler = CreateHandler();

        // ajuste este construtor conforme a sua entidade Loan
        var loan = new Loan(
            gameId: 1,
            friendId: 1,
            loanDate: DateTime.UtcNow.AddDays(-3),
            expectedReturnDate: DateTime.UtcNow.AddDays(4)
        );
        loan.RegisterReturn(DateTime.UtcNow); // deixa como "fechado"

        _loanRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loan);

        var command = new RegisterReturnCommand(1);

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Only open loans can be returned*");
    }

   
}

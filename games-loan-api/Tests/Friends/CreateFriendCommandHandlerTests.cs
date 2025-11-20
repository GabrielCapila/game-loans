using FluentAssertions;
using GamesLoan.Application.Friends.Command.CreateFriend;
using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using Moq;

namespace GamesLoan.Tests.Friends;

public class CreateFriendCommandHandlerTests
{
    private readonly Mock<IFriendRepository> _friendRepo = new();

    private CreateFriendCommandHandler CreateHandler()
        => new(_friendRepo.Object);

    [Fact]
    public async Task Should_throw_DomainException_when_email_is_invalid()
    {
        var handler = CreateHandler();

        var command = new CreateFriendCommand(
            Name: "Gabriel",
            Email: "email_invalido",
            Phone: "31999999999"
        );

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid e-mail format*");
    }

    [Fact]
    public async Task Should_throw_DomainException_when_phone_is_invalid()
    {
        var handler = CreateHandler();

        var command = new CreateFriendCommand(
            Name: "Gabriel",
            Email: "gabriel@test.com",
            Phone: "abc"
        );

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid phone number format*");
    }

    [Fact]
    public async Task Should_throw_DomainException_when_email_already_exists()
    {
        var handler = CreateHandler();

        _friendRepo.Setup(r => r.ExistsByEmailAsync("gabriel@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateFriendCommand(
            Name: "Gabriel",
            Email: "gabriel@test.com",
            Phone: "31999999999"
        );

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("A friend with this email already exists.");
    }

    [Fact]
    public async Task Should_create_friend_successfully()
    {
        var handler = CreateHandler();

        _friendRepo.Setup(r => r.ExistsByEmailAsync("gabriel@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _friendRepo.Setup(r => r.AddAsync(It.IsAny<Friend>(), It.IsAny<CancellationToken>()))
            .Callback<Friend, CancellationToken>((f, _) => { /* poderia setar Id aqui se quisesse */ })
            .Returns(Task.CompletedTask);

        var command = new CreateFriendCommand(
            Name: "Gabriel",
            Email: "gabriel@test.com",
            Phone: "31999999999"
        );

        var id = await handler.Handle(command, CancellationToken.None);

        _friendRepo.Verify(r => r.AddAsync(It.IsAny<Friend>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

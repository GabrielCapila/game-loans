using GamesLoan.Application.Friends.Command.CreateFriend;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using Moq;

public class CreateFriendCommandHandlerTests
{
    [Fact]
    public async Task Should_throw_when_email_is_invalid()
    {
        var friendRepo = new Mock<IFriendRepository>();
        var handler = new CreateFriendCommandHandler(friendRepo.Object);

        var command = new CreateFriendCommand(
            Name: "Gabriel",
            Email: "email_invalido",
            Phone: "31999999999"
        );

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}

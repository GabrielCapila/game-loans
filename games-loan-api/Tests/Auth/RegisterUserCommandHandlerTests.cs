using FluentAssertions;
using GamesLoan.Application.Auth.Commands.RegisterUser;
using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace GamesLoan.Tests.Auth;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    private RegisterUserCommandHandler CreateHandler()
        => new(_userRepo.Object, _passwordHasher);

    [Fact]
    public async Task Should_throw_when_username_already_exists()
    {
        var handler = CreateHandler();

        _userRepo.Setup(r => r.ExistsByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterUserCommand("admin", "123456");

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Username 'admin' is already taken*");
    }

    [Fact]
    public async Task Should_register_user_with_hashed_password()
    {
        var handler = CreateHandler();
        User? capturedUser = null;

        _userRepo.Setup(r => r.ExistsByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => capturedUser = u)
            .Returns(Task.CompletedTask);

        var command = new RegisterUserCommand("admin", "123456");

        var id = await handler.Handle(command, CancellationToken.None);

        capturedUser.Should().NotBeNull();
        capturedUser!.PasswordHash.Should().NotBeNullOrWhiteSpace();
        capturedUser.PasswordHash.Should().NotBe("123456");
    }
}

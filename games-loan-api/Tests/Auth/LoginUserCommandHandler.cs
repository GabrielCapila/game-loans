using FluentAssertions;
using GamesLoan.Application.Auth.Commands.LoginUser;
using GamesLoan.Application.Auth.Models;
using GamesLoan.Application.Auth.Services;
using GamesLoan.Application.Exceptions;
using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace GamesLoan.Tests.Auth;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
    private readonly Mock<ITokenService> _tokenService = new();

    private LoginUserCommandHandler CreateHandler()
        => new(_userRepo.Object, _passwordHasher, _tokenService.Object);

    [Fact]
    public async Task Should_throw_NotFound_when_user_does_not_exist()
    {
        var handler = CreateHandler();

        _userRepo.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginUserCommand("admin", "123456");

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Invalid username or password*");
    }

  
    [Fact]
    public async Task Should_return_token_and_username_on_success()
    {
        var handler = CreateHandler();

        var user = new User("admin");
        var hash = _passwordHasher.HashPassword(user, "123456");
        user.SetPasswordHash(hash);

        _userRepo.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _tokenService.Setup(t => t.GenerateToken(user.Id, user.Username))
            .Returns("fake-jwt-token");

        var command = new LoginUserCommand("admin", "123456");

        // act
        LoginResponse result = await handler.Handle(command, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.Username.Should().Be("admin");
        result.Token.Should().Be("fake-jwt-token");
    }
}

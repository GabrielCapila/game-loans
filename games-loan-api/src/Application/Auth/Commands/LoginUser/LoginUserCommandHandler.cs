using GamesLoan.Application.Auth.Models;
using GamesLoan.Application.Auth.Services;
using GamesLoan.Application.Exceptions;
using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Auth.Commands.LoginUser;
public sealed class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken)
            ?? throw new NotFoundException("Invalid username or password.");

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verifyResult == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid username or password.");

        var token = _tokenService.GenerateToken(user.Id, user.Username);

        return new LoginResponse
        {
            Token = token,
            Username = user.Username
        };
    }
}

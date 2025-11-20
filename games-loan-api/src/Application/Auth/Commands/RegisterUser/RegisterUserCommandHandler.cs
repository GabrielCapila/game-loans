using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Auth.Commands.RegisterUser;
public sealed class RegisterUserCommandHandler
    : IRequestHandler<RegisterUserCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByUsernameAsync(request.Username, cancellationToken))
            throw new DomainException($"Username '{request.Username}' is already taken.");

        var user = new User(request.Username);

        var hash = _passwordHasher.HashPassword(user, request.Password);

        user.SetPasswordHash(hash);

        await _userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}
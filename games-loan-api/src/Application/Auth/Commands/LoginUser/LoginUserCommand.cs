using GamesLoan.Application.Auth.Models;
using MediatR;

namespace GamesLoan.Application.Auth.Commands.LoginUser;
public sealed record LoginUserCommand(string Username, string Password)
    : IRequest<LoginResponse>;
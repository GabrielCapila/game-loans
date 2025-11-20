using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Auth.Commands.RegisterUser;
public sealed record RegisterUserCommand(string Username, string Password) : IRequest<int>;

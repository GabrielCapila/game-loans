using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Games.Commands.CreateGame;
public sealed record CreateGameCommand(
    string Name,
    List<string>? Publishers,
    List<string>? Genre,
    string? ExternalSourceId
) : IRequest<int>;
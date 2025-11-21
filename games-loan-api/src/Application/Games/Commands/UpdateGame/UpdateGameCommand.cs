using MediatR;
using System.Collections.Generic;

namespace GamesLoan.Application.Games.Commands.UpdateGame;
public sealed record UpdateGameCommand(
    int Id,
    string Name,
    List<string>? Publishers,
    List<string>? Genre
) : IRequest<bool>;
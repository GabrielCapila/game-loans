using GamesLoan.Application.Games.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Games.Queries.ListGames;
public sealed record ListGamesQuery() : IRequest<IReadOnlyList<GameDto>>;

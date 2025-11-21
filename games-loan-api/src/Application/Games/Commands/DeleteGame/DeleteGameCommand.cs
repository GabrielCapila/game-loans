using MediatR;

namespace GamesLoan.Application.Games.Commands.DeleteGame;
public sealed record DeleteGameCommand(int Id) : IRequest<bool>;
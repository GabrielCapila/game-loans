using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Queries.UpdateFriend;
public sealed record UpdateFriendCommand(
    int Id,
    string Name,
    string? Email,
    string? Phone
) : IRequest<bool>;
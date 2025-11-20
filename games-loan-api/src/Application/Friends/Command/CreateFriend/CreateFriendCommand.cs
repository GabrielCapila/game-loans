using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Command.CreateFriend;
public sealed record CreateFriendCommand(
    string Name,
    string Email,
    string Phone
) : IRequest<int>;


using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Queries.DeleteFriend;
public sealed record DeleteFriendCommand(int Id) : IRequest<bool>;
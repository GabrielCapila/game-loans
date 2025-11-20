using GamesLoan.Application.Friends.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Queries.ListFriends;
public sealed record ListFriendsQuery() : IRequest<IReadOnlyList<FriendDto>>;
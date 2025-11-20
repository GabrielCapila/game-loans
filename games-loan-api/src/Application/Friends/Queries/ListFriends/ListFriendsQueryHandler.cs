using GamesLoan.Application.Friends.Models;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Queries.ListFriends;
public sealed class ListFriendsQueryHandler
    : IRequestHandler<ListFriendsQuery, IReadOnlyList<FriendDto>>
{
    private readonly IFriendRepository _friendRepository;

    public ListFriendsQueryHandler(IFriendRepository friendRepository)
    {
        _friendRepository = friendRepository;
    }

    public async Task<IReadOnlyList<FriendDto>> Handle(ListFriendsQuery request, CancellationToken cancellationToken)
    {
        var friends = await _friendRepository.GetAllAsync(cancellationToken);

        return friends
            .Select(f => new FriendDto
            {
                Id = f.Id,
                Name = f.Name,
                Email = f.Email,
                Phone = f.Phone,
                CreatedAt = f.CreatedAt,
                IsActive = f.IsActive
            })
            .ToList();
    }
}

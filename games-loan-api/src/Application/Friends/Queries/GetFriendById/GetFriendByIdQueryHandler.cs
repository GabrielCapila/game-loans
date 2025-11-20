using GamesLoan.Application.Friends.Models;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Queries.GetFriendById;
public sealed class GetFriendByIdQueryHandler
    : IRequestHandler<GetFriendByIdQuery, FriendDto?>
{
    private readonly IFriendRepository _friendRepository;

    public GetFriendByIdQueryHandler(IFriendRepository friendRepository)
    {
        _friendRepository = friendRepository;
    }

    public async Task<FriendDto?> Handle(GetFriendByIdQuery request, CancellationToken cancellationToken)
    {
        var friend = await _friendRepository.GetByIdAsync(request.Id, cancellationToken);

        if (friend is null)
            return null;

        return new FriendDto
        {
            Id = friend.Id,
            Name = friend.Name,
            Email = friend.Email,
            Phone = friend.Phone,
            CreatedAt = friend.CreatedAt,
            IsActive = friend.IsActive
        };
    }
}

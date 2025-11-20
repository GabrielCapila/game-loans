using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Queries.UpdateFriend;
public sealed class UpdateFriendCommandHandler
    : IRequestHandler<UpdateFriendCommand, bool>
{
    private readonly IFriendRepository _friendRepository;

    public UpdateFriendCommandHandler(IFriendRepository friendRepository)
    {
        _friendRepository = friendRepository;
    }

    public async Task<bool> Handle(UpdateFriendCommand request, CancellationToken cancellationToken)
    {
        var friend = await _friendRepository.GetByIdAsync(request.Id, cancellationToken);

        if (friend is null)
            return false;

        friend.SetName(request.Name);
        friend.SetEmail(request.Email);
        friend.SetPhone(request.Phone);

        await _friendRepository.UpdateAsync(friend, cancellationToken);

        return true;
    }
}
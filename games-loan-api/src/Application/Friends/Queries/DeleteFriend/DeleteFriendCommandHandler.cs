using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Queries.DeleteFriend;
public sealed class DeleteFriendCommandHandler
    : IRequestHandler<DeleteFriendCommand, bool>
{
    private readonly IFriendRepository _friendRepository;

    public DeleteFriendCommandHandler(IFriendRepository friendRepository)
    {
        _friendRepository = friendRepository;
    }

    public async Task<bool> Handle(DeleteFriendCommand request, CancellationToken cancellationToken)
    {
        var friend = await _friendRepository.GetByIdAsync(request.Id, cancellationToken);

        if (friend is null)
            return false;

        // soft-delete
        friend.Deactivate();
        await _friendRepository.UpdateAsync(friend, cancellationToken);

        // se quiser hard delete, troque por:
        // await _friendRepository.DeleteAsync(friend, cancellationToken);

        return true;
    }
}
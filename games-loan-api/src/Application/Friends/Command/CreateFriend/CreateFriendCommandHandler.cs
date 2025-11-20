using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Command.CreateFriend;
public sealed class CreateFriendCommandHandler
    : IRequestHandler<CreateFriendCommand, int>
{
    private readonly IFriendRepository _friendRepository;

    public CreateFriendCommandHandler(IFriendRepository friendRepository)
    {
        _friendRepository = friendRepository;
    }

    public async Task<int> Handle(CreateFriendCommand request, CancellationToken cancellationToken)
    {

        if (!string.IsNullOrWhiteSpace(request.Email) && !IsValidEmail(request.Email))
            throw new DomainException("Invalid e-mail format.");

        if (!string.IsNullOrWhiteSpace(request.Phone) && !IsValidPhone(request.Phone))
            throw new DomainException("Invalid phone number format.");

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var exists = await _friendRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (exists)
                throw new DomainException($"A friend with this email already exists.");
        }

        var friend = new Friend(request.Name, request.Email, request.Phone);

        await _friendRepository.AddAsync(friend, cancellationToken);

        return friend.Id;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var _ = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch { return false; }
    }

    private static bool IsValidPhone(string phone)
    {
        return Regex.IsMatch(phone, @"^\+?[0-9\s\-().]{8,20}$");
    }
}

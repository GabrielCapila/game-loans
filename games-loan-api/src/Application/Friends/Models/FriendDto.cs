using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Friends.Models;
public sealed class FriendDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsActive { get; init; }
}
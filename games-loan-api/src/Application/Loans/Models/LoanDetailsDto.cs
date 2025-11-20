using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Models;
public sealed class LoanDetailsDto
{
    public int Id { get; init; }

    public int FriendId { get; init; }
    public string FriendName { get; init; } = null!;

    public int GameId { get; init; }
    public string GameTitle { get; init; } = null!;

    public DateTime LoanDate { get; init; }
    public DateTime? ExpectedReturnDate { get; init; }
    public DateTime? ReturnDate { get; init; }

    public string Status { get; init; } = null!;
}
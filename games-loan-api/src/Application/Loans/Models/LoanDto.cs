using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Models;
public sealed class LoanDto
{
    public int Id { get; init; }

    public int FriendId { get; init; }
    public int GameId { get; init; }

    public DateTime LoanDate { get; init; }
    public DateTime? ExpectedReturnDate { get; init; }
    public DateTime? ReturnDate { get; init; }

    // Ex: "Open", "Returned", "Cancelled"
    public string Status { get; init; } = null!;
}
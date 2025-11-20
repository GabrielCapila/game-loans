using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Models;
public sealed class LoanCreatedDto
{
    public int LoanId { get; init; }
    public int FriendId { get; init; }
    public int GameId { get; init; }
    public string Message { get; init; } = null!;
}

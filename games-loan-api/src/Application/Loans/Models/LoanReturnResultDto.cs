using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Models;
public sealed class LoanReturnResultDto
{
    public int LoanId { get; init; }
    public int FriendId { get; init; }
    public string FriendName { get; init; } = null!;
    public int GameId { get; init; }
    public string GameTitle { get; init; } = null!;
    public string Message { get; init; } = null!;
}
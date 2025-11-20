using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Models;
public sealed class LoanUpdatedDto
{
    public int Id { get; init; }
    public string Message { get; init; } = string.Empty;
    public DateTime ExpectedReturnDate { get; init; }
}
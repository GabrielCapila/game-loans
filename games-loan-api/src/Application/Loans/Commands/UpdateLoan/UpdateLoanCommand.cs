using GamesLoan.Application.Loans.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Commands.UpdateLoan;
public sealed record UpdateLoanCommand(
    int LoanId,
    DateTime ExpectedReturnDate
) : IRequest<LoanUpdatedDto>;
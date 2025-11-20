using GamesLoan.Application.Loans.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Queries.ListLoans;
public sealed record ListLoansQuery(bool OnlyActive = false)
    : IRequest<IReadOnlyList<LoanDetailsDto>>;
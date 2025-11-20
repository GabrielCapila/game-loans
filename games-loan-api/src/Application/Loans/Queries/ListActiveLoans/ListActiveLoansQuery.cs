using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesLoan.Application.Loans.Models;

namespace GamesLoan.Application.Loans.Queries.ListActiveLoans;

public sealed record ListActiveLoansQuery() : IRequest<IReadOnlyList<LoanDto>>;
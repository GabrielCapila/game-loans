using GamesLoan.Application.Loans.Models;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Queries.ListActiveLoans;
public sealed class ListActiveLoansQueryHandler
    : IRequestHandler<ListActiveLoansQuery, IReadOnlyList<LoanDto>>
{
    private readonly ILoanRepository _loanRepository;

    public ListActiveLoansQueryHandler(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<IReadOnlyList<LoanDto>> Handle(
        ListActiveLoansQuery request,
        CancellationToken cancellationToken)
    {
        var loans = await _loanRepository.GetOpenLoansAsync(cancellationToken);

        return loans.Select(l => new LoanDto
        {
            Id = l.Id,
            FriendId = l.FriendId,
            GameId = l.GameId,
            LoanDate = l.LoanDate,
            ExpectedReturnDate = l.ExpectedReturnDate,
            Status = l.Status.ToString()
        }).ToList();
    }
}
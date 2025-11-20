using GamesLoan.Application.Exceptions;
using GamesLoan.Application.Loans.Models;
using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Loans.Commands.UpdateLoan;
public sealed class UpdateLoanCommandHandler
    : IRequestHandler<UpdateLoanCommand, LoanUpdatedDto>
{
    private readonly ILoanRepository _loanRepo;

    public UpdateLoanCommandHandler(ILoanRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<LoanUpdatedDto> Handle(UpdateLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepo.GetByIdAsync(request.LoanId, cancellationToken)
            ?? throw new NotFoundException($"Loan with id {request.LoanId} was not found.");

        if (request.ExpectedReturnDate == default)
            throw new DomainException("Expected return date is required.");

        loan.UpdateExpectedReturnDate(request.ExpectedReturnDate);

        // 4. persiste
        await _loanRepo.UpdateAsync(loan, cancellationToken);

        return new LoanUpdatedDto
        {
            Id = loan.Id,
            ExpectedReturnDate = loan.ExpectedReturnDate,
            Message = $"Expected return date for this loan was updated to {loan.ExpectedReturnDate:dd/MM/yyyy}."
        };
    }
}
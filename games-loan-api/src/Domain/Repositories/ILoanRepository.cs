using GamesLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Domain.Repositories;
public interface ILoanRepository : IBaseRepository<Loan>
{

    // Usado pra saber se o jogo já está emprestado, se quiser consultar direto
    Task<bool> HasOpenLoanForGameAsync(int gameId, CancellationToken cancellationToken = default);

    // Consultas úteis para queries depois
    Task<IReadOnlyList<Loan>> GetOpenLoansAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Loan>> GetLoansByFriendAsync(int friendId, CancellationToken cancellationToken = default);
}

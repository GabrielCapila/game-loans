using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using GamesLoan.Domain.Types;
using Microsoft.EntityFrameworkCore;

namespace GamesLoan.Infrastructure.Persistence.Repositories;
public class LoanRepository : BaseRepository<Loan>, ILoanRepository
{
    public LoanRepository(GamesLoanDbContext context)
        : base(context)
    {
    }

    public Task<bool> HasOpenLoanForGameAsync(int gameId, CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(
            l => l.GameId == gameId && l.Status == LoanStatus.Open,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Loan>> GetOpenLoansAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(l => l.Status == LoanStatus.Open)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Loan>> GetLoansByFriendAsync(
        int friendId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(l => l.FriendId == friendId)
            .ToListAsync(ct);
    }
}

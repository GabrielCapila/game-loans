using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GamesLoan.Infrastructure.Persistence.Repositories;
public class GameRepository : BaseRepository<Game>, IGameRepository
{
    public GameRepository(GamesLoanDbContext context)
        : base(context)
    {
    }

    public Task<Game?> GetByExternalSourceIdAsync(
        string externalSourceId,
        CancellationToken cancellationToken = default)
    {
        return _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.ExternalSourceId == externalSourceId, cancellationToken);
    }
}

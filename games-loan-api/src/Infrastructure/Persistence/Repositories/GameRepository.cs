using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GamesLoan.Infrastructure.Persistence.Repositories;
public class GameRepository : BaseRepository<Game>, IGameRepository
{
    private readonly GamesLoanDbContext _db;
    public GameRepository(GamesLoanDbContext context)
        : base(context)
    {
        _db = context;
    }

    public Task<Game?> GetByExternalSourceIdAsync(
        string externalSourceId,
        CancellationToken cancellationToken = default)
    {
        return _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.ExternalSourceId == externalSourceId, cancellationToken);
    }
    public async Task<HashSet<string>> GetExistingExternalSourceIdsAsync(
       IEnumerable<string> externalSourceIds,
       CancellationToken cancellationToken = default)
    {
        var ids = externalSourceIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        if (ids.Count == 0)
            return new HashSet<string>();

        var existing = await _db.Games
            .AsNoTracking()
            .Where(g => ids.Contains(g.ExternalSourceId))
            .Select(g => g.ExternalSourceId)
            .ToListAsync(cancellationToken);

        return existing.ToHashSet();
    }

    public async Task AddRangeAsync(IEnumerable<Game> games, CancellationToken cancellationToken = default)
    {
        var list = games.ToList();
        if (list.Count == 0)
            return;

        await _db.Games.AddRangeAsync(list, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

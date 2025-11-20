using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using GamesLoan.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Persistence.Configurations;
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(GamesLoanDbContext context) : base(context) { }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => _dbSet.FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
        => _dbSet.AnyAsync(u => u.Username == username, ct);
}
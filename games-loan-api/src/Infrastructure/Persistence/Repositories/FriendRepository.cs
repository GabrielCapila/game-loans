using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Persistence.Repositories;
public class FriendRepository : BaseRepository<Friend>, IFriendRepository
{
    public FriendRepository(GamesLoanDbContext context)
        : base(context) { }
    
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(f => f.Email == email, cancellationToken);
    }
}

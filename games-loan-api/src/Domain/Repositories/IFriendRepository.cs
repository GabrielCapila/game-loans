using GamesLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Domain.Repositories;
public interface IFriendRepository : IBaseRepository<Friend>
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

}

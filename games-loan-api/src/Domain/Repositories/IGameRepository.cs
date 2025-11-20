using GamesLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Domain.Repositories;
public interface IGameRepository : IBaseRepository<Game>
{
    // Para evitar duplicar carga da API externa
    Task<Game?> GetByExternalSourceIdAsync(string externalSourceId, CancellationToken cancellationToken = default);

}

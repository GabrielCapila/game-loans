using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Persistence;
public class GamesLoanDbContextFactory : IDesignTimeDbContextFactory<GamesLoanDbContext>
{
    public GamesLoanDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GamesLoanDbContext>();

        // mesma connection do docker-compose
        var connectionString =
            "server=localhost;port=3306;database=gamesloan;user=gamesuser;password=gamespass";

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new GamesLoanDbContext(optionsBuilder.Options);
    }
}

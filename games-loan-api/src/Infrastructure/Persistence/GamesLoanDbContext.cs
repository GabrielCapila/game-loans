using GamesLoan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Persistence;
public class GamesLoanDbContext : DbContext
{
    public GamesLoanDbContext(DbContextOptions<GamesLoanDbContext> options)
        : base(options)
    {
    }

    public DbSet<Friend> Friends => Set<Friend>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GamesLoanDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

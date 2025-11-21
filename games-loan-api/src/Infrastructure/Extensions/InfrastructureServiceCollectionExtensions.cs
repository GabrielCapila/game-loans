using GamesLoan.Application.Auth.Services;
using GamesLoan.Domain.Repositories;
using GamesLoan.Infrastructure.Auth;
using GamesLoan.Infrastructure.Integrations.Playstation;
using GamesLoan.Infrastructure.Jobs;
using GamesLoan.Infrastructure.Persistence;
using GamesLoan.Infrastructure.Persistence.Configurations;
using GamesLoan.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Extensions;
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<GamesLoanDbContext>(options =>
        {
            options
           .UseMySql(
               connectionString,
               ServerVersion.AutoDetect(connectionString))
           .EnableSensitiveDataLogging(false)
           .EnableDetailedErrors(true)
           .LogTo(Console.WriteLine, LogLevel.Error);
        });



    services.AddScoped<IFriendRepository, FriendRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPlaystationGamesClient, PlaystationGamesClient>();
        services.AddHostedService<GamesImportHostedService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddHttpClient("PlaystationApi", client =>
        {
            client.BaseAddress = new Uri("https://api.sampleapis.com/playstation/");
    client.Timeout = TimeSpan.FromSeconds(15);
        });

        return services;
    }
}

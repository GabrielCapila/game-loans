using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using GamesLoan.Infrastructure.Integrations.Playstation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Jobs;
public sealed class GamesImportHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GamesImportHostedService> _logger;

    public GamesImportHostedService(
        IServiceProvider serviceProvider,
        ILogger<GamesImportHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting games import job...");

        using var scope = _serviceProvider.CreateScope();
        var gamesClient = scope.ServiceProvider.GetRequiredService<IPlaystationGamesClient>();
        var gameRepository = scope.ServiceProvider.GetRequiredService<IGameRepository>();

        var externalGames = await gamesClient.GetGamesAsync(cancellationToken);

        var importedCount = 0;

        foreach (var external in externalGames)
        {
            if (string.IsNullOrWhiteSpace(external.Name))
                continue;

            var externalId = external.Id.ToString();

            var existing = await gameRepository.GetByExternalSourceIdAsync(
                externalId, cancellationToken);

            if (existing is not null)
                continue; 

            var game = new Game(
                name: external.Name,
                publishers: external.Publishers,
                genre: external.Genre,
                externalSourceId: externalId
            );

            await gameRepository.AddAsync(game, cancellationToken);
            importedCount++;
        }

        _logger.LogInformation("Games import finished. Imported {Count} games.", importedCount);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

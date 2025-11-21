using GamesLoan.Domain.Entities;
using GamesLoan.Domain.Repositories;
using GamesLoan.Infrastructure.Integrations.Playstation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting games import job...");

        using var scope = _serviceProvider.CreateScope();
        var gamesClient = scope.ServiceProvider.GetRequiredService<IPlaystationGamesClient>();
        var gameRepository = scope.ServiceProvider.GetRequiredService<IGameRepository>();

        var externalGames = await gamesClient.GetGamesAsync(cancellationToken);

        var externalIds = externalGames
            .Where(e => !string.IsNullOrWhiteSpace(e.Name))
            .Select(e => e.Id.ToString())
            .ToList();

        var existingIds = await gameRepository
            .GetExistingExternalSourceIdsAsync(externalIds, cancellationToken);

        var newGames = new List<Game>();

        foreach (var external in externalGames)
        {
            if (string.IsNullOrWhiteSpace(external.Name))
                continue;

            var externalId = external.Id.ToString();

            if (existingIds.Contains(externalId))
                continue;

            var game = new Game(
                name: external.Name,
                publishers: external.Publishers ?? new List<string>(),
                genre: external.Genre ?? new List<string>(),
                externalSourceId: externalId
            );

            newGames.Add(game);
        }

        await gameRepository.AddRangeAsync(newGames, cancellationToken);

        stopwatch.Stop();

        _logger.LogInformation(
            "Games import finished. Imported {ImportedCount} new games out of {TotalExternal}. Duration: {ElapsedMs} ms.",
            newGames.Count,
            externalGames.Count,
            stopwatch.ElapsedMilliseconds
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

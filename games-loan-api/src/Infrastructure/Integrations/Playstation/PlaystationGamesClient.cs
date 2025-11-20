using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Integrations.Playstation;
public interface IPlaystationGamesClient
{
    Task<IReadOnlyList<PlaystationGameExternalDto>> GetGamesAsync(
        CancellationToken cancellationToken = default);
}

public sealed class PlaystationGamesClient : IPlaystationGamesClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PlaystationGamesClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IReadOnlyList<PlaystationGameExternalDto>> GetGamesAsync(
        CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("PlaystationApi");

        var result = await client.GetFromJsonAsync<IReadOnlyList<PlaystationGameExternalDto>>(
            "games", cancellationToken);

        return result ?? Array.Empty<PlaystationGameExternalDto>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamesLoan.Infrastructure.Integrations.Playstation;
public sealed class PlaystationGameExternalDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("genre")]
    public List<string>? Genre { get; set; }
    [JsonPropertyName("publishers")]
    public List<string>? Publishers { get; set; }
}
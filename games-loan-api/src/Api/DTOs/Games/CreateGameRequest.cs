using System.ComponentModel.DataAnnotations;

namespace GamesLoan.Api.DTOs.Games;

public sealed class CreateGameRequest
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public List<string> Publishers { get; set; } = new();

    [Required]
    public List<string> Genre { get; set; } = new();

    public string? ExternalSourceId { get; set; }
}
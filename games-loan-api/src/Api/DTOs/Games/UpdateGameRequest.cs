namespace GamesLoan.Api.DTOs.Games;
public sealed class UpdateGameRequest
{
    public string Name { get; set; } = null!;
    public List<string>? Publishers { get; set; }
    public List<string>? Genre { get; set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Application.Games.Models;
public sealed class GameDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public List<string>? Publishers { get; init; }
    public List<string>? Genre { get; init; }
    public string? ExternalSourceId { get; init; }
    public bool IsLoaned { get; init; }
    public DateTime CreatedAt { get; init; }
}
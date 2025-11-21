using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Domain.Entities;
public record Game
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public List<string>? Publishers { get; private set; }
    public List<string>? Genre { get; private set; }

    // Id vindo da API externa (SampleApis)
    public string ExternalSourceId { get; private set; }

    public bool IsLoaned { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Game() { }

    public Game(string name, List<string> publishers, List<string> genre, string externalSourceId)
    {
        SetName(name);
        Publishers = publishers;
        Genre = genre;
        ExternalSourceId = externalSourceId;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name.Trim();
    }

    public void SetPublishers(List<string>? publishers)
    {
        Publishers = publishers?.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).ToList();
    }

    public void SetGenre(List<string>? genre)
    {
        Genre = genre?.Where(g => !string.IsNullOrWhiteSpace(g)).Select(g => g.Trim()).ToList();
    }

    public void MarkAsLoaned()
    {
        if (IsLoaned)
            throw new InvalidOperationException("Game is already loaned.");

        IsLoaned = true;
    }

    public void MarkAsReturned()
    {
        if (!IsLoaned)
            throw new InvalidOperationException("Game is not currently loaned.");

        IsLoaned = false;
    }
}

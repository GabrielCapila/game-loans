using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesLoan.Domain.Entities;
public record Friend
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    private Friend() { }

    public Friend(string name, string? email = null, string? phone = null)
    {
        SetName(name);
        SetEmail(email);
        Phone = phone;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name.Trim();
    }

    public void SetEmail(string? email)
    {
        if (email is not null && string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        Email = email?.Trim();
    }

    public void SetPhone(string? phone)
    {
        Phone = phone?.Trim();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
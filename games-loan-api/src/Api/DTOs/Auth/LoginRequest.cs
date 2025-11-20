namespace GamesLoan.Api.DTOs.Auth;

public sealed class LoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
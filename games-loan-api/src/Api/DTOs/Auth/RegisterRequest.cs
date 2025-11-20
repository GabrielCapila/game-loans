namespace GamesLoan.Api.DTOs.Auth;

public sealed class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
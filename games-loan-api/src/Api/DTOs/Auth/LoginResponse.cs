namespace GamesLoan.Api.DTOs.Auth;

public sealed class LoginResponse
{
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
}
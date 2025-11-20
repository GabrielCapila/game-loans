using System.ComponentModel.DataAnnotations;

namespace GamesLoan.Api.DTOs.Friends;

public sealed class CreateFriendRequest
{
    [Required]
    public string Name { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Invalid e-mail format.")]
    public string? Email { get; set; }

    [RegularExpression(@"^\+?[0-9\s\-().]{8,20}$",
        ErrorMessage = "Invalid phone number format.")]
    public string? Phone { get; set; }
}
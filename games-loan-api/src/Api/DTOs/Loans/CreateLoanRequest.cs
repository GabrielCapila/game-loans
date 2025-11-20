using System.ComponentModel.DataAnnotations;

namespace GamesLoan.Api.DTOs.Loans;

public sealed class CreateLoanRequest
{
    public int FriendId { get; set; }
    public int GameId { get; set; }
    [Required(ErrorMessage = "Expected return date is required.")]
    public DateTime ExpectedReturnDate { get; set; }
}
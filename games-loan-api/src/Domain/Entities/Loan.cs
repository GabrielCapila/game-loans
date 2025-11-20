using GamesLoan.Domain.Exceptions;
using GamesLoan.Domain.Types;

namespace GamesLoan.Domain.Entities;
public record Loan
{
    public int Id { get; private set; }
    public int GameId { get; private set; }
    public int FriendId { get; private set; }
    public LoanStatus Status { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime ExpectedReturnDate { get; private set; }
    public DateTime? ActualReturnDate { get; private set; }

    public bool IsReturned => ActualReturnDate.HasValue;
    private Loan() { }

    public Loan(int gameId, int friendId, DateTime loanDate, DateTime expectedReturnDate)
    {
        if (gameId <= 0)
            throw new ArgumentException("GameId is required.", nameof(gameId));

        if (friendId <= 0)
            throw new ArgumentException("FriendId is required.", nameof(friendId));

        GameId = gameId;
        FriendId = friendId;
        LoanDate = loanDate;
        ExpectedReturnDate = expectedReturnDate;
        Status = LoanStatus.Open;
    }

    public void RegisterReturn(DateTime returnDate)
    {
        if (Status != LoanStatus.Open)
            throw new InvalidOperationException("Only open loans can be returned.");

        if (returnDate < LoanDate)
            throw new ArgumentException("Return date cannot be before loan date.", nameof(returnDate));

        ActualReturnDate = returnDate;
        Status = LoanStatus.Returned;
    }

    public void Cancel()
    {
        if (Status != LoanStatus.Open)
            throw new InvalidOperationException("Only open loans can be cancelled.");

        Status = LoanStatus.Cancelled;
    }

    public void UpdateExpectedReturnDate(DateTime newExpectedReturnDate)
    {
        if (IsReturned)
            throw new DomainException("Cannot change the expected return date of a returned loan.");

        if (newExpectedReturnDate <= LoanDate)
            throw new DomainException("Expected return date must be after the loan date.");

        ExpectedReturnDate = newExpectedReturnDate;
    }
}

namespace LoanHub.Api.Entities;

public class Customer : EntityBase
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string PersonalNumber { get; set; }

    public DateOnly BirthDate { get; set; }

    public int CreditScore { get; set; }

    public List<Loan> Loans { get; set; } = new();
}

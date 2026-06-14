namespace LoanHub.Api.Entities;

public class Payment : EntityBase
{
    public int LoanId { get; set; }

    public Loan? Loan { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }
}

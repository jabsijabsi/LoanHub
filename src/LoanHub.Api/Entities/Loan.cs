namespace LoanHub.Api.Entities;

public class Loan : EntityBase
{
    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public decimal Amount { get; set; }

    /// <summary>Annual interest rate as a percentage (e.g. 13.5 means 13.5%).</summary>
    public decimal InterestRate { get; set; }

    public int TermMonths { get; set; }

    public decimal MonthlyPayment { get; set; }

    public LoanState Status { get; set; } = LoanState.Pending;

    public List<Payment> Payments { get; set; } = new();

    public List<LoanSchedule> ScheduleItems { get; set; } = new();
}

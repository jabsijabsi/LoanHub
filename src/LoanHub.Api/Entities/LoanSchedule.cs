namespace LoanHub.Api.Entities;

/// <summary>One planned installment of a loan's repayment plan.</summary>
public class LoanSchedule : EntityBase
{
    public int LoanId { get; set; }

    public Loan? Loan { get; set; }

    public int Sequence { get; set; }

    /// <summary>Planned payment amount (PMT) for this installment.</summary>
    public decimal PMT { get; set; }

    public DateTime Date { get; set; }
}

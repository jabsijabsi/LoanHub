namespace LoanHub.Api.Contracts;

public record LoanApplicationRequest(
    int CustomerId,
    decimal Amount,
    decimal InterestRate,
    int TermMonths);

public record ScheduleItemView(int Sequence, decimal PMT, DateTime Date);

public class LoanView
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public decimal MonthlyPayment { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Remaining { get; set; }
    public List<ScheduleItemView> Schedule { get; set; } = new();
}

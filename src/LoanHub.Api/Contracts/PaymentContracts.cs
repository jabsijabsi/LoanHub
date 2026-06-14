namespace LoanHub.Api.Contracts;

public record PaymentRequest(int LoanId, decimal Amount, DateTime? PaymentDate);

public record PaymentReceipt(
    int PaymentId,
    int LoanId,
    decimal Amount,
    DateTime PaymentDate,
    decimal TotalPaid,
    decimal Remaining,
    string Status);

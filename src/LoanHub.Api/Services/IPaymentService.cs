using LoanHub.Api.Contracts;

namespace LoanHub.Api.Services;

public interface IPaymentService
{
    Task<PaymentReceipt> PayAsync(PaymentRequest request, CancellationToken ct = default);
}

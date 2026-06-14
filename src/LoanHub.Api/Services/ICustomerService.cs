using LoanHub.Api.Contracts;

namespace LoanHub.Api.Services;

public interface ICustomerService
{
    Task<CustomerView> CreateAsync(NewCustomerRequest request, CancellationToken ct = default);

    Task<CustomerView> GetAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<LoanView>> GetLoanHistoryAsync(int customerId, CancellationToken ct = default);
}

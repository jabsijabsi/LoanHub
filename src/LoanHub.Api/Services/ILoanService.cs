using LoanHub.Api.Contracts;

namespace LoanHub.Api.Services;

public interface ILoanService
{
    Task<LoanView> ApplyAsync(LoanApplicationRequest request, CancellationToken ct = default);

    Task<LoanView> GetAsync(int id, CancellationToken ct = default);
}

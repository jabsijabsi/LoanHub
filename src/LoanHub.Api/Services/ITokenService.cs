using LoanHub.Api.Contracts;

namespace LoanHub.Api.Services;

public interface ITokenService
{
    TokenResponse Issue(string username);
}

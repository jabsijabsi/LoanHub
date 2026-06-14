using LoanHub.Api.Contracts;

namespace LoanHub.Api.Services;

public interface IAuthService
{
    TokenResponse Login(TokenRequest request);
}

using LoanHub.Api.Common;
using LoanHub.Api.Contracts;
using Microsoft.Extensions.Options;

namespace LoanHub.Api.Services;

/// <summary>
/// Minimal demo auth: checks one configured credential pair and issues a JWT.
/// Swap for a real user store in production.
/// </summary>
public class AuthService : IAuthService
{
    private readonly JwtOptions _options;
    private readonly ITokenService _tokenService;

    public AuthService(IOptions<JwtOptions> options, ITokenService tokenService)
    {
        _options = options.Value;
        _tokenService = tokenService;
    }

    public TokenResponse Login(TokenRequest request)
    {
        var ok = request.Username == _options.DemoUsername && request.Password == _options.DemoPassword;
        if (!ok)
        {
            throw new AuthException("Invalid username or password.");
        }

        return _tokenService.Issue(request.Username);
    }
}

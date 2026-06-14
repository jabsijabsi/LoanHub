using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoanHub.Api.Common;
using LoanHub.Api.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LoanHub.Api.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly IClock _clock;

    public TokenService(IOptions<JwtOptions> options, IClock clock)
    {
        _options = options.Value;
        _clock = clock;
    }

    public TokenResponse Issue(string username)
    {
        var expiresAt = _clock.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, username),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: _clock.UtcNow,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        var encoded = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new TokenResponse(encoded, expiresAt);
    }
}

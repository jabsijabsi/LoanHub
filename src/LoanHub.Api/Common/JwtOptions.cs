namespace LoanHub.Api.Common;

public class JwtOptions
{
    public const string Section = "JwtConfig";

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 90;

    public string DemoUsername { get; set; } = "AniJabakhidze";
    public string DemoPassword { get; set; } = "Password1!";
}

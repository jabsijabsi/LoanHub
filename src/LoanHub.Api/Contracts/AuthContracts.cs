namespace LoanHub.Api.Contracts;

public record TokenRequest(string Username, string Password);

public record TokenResponse(string Token, DateTime ExpiresAtUtc);

namespace LoanHub.Api.Contracts;

public record NewCustomerRequest(
    string FirstName,
    string LastName,
    string PersonalNumber,
    DateOnly BirthDate,
    int CreditScore);

public record CustomerView(
    int Id,
    string FirstName,
    string LastName,
    string PersonalNumber,
    DateOnly BirthDate,
    int CreditScore);

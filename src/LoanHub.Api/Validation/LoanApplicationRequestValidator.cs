using FluentValidation;
using LoanHub.Api.Contracts;

namespace LoanHub.Api.Validation;

public class LoanApplicationRequestValidator : AbstractValidator<LoanApplicationRequest>
{
    public LoanApplicationRequestValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);

        RuleFor(x => x.Amount)
            .InclusiveBetween(500m, 50_000m)
            .WithMessage("Amount must be between 500 and 50,000.");

        RuleFor(x => x.TermMonths)
            .InclusiveBetween(6, 60)
            .WithMessage("Term must be between 6 and 60 months.");

        RuleFor(x => x.InterestRate).InclusiveBetween(0m, 100m);
    }
}

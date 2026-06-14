using FluentValidation;
using LoanHub.Api.Contracts;

namespace LoanHub.Api.Validation;

public class NewCustomerRequestValidator : AbstractValidator<NewCustomerRequest>
{
    public NewCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.PersonalNumber).NotEmpty().MaximumLength(40);

        RuleFor(x => x.BirthDate)
            .Must(d => d < DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("BirthDate must be in the past.");

        RuleFor(x => x.CreditScore).InclusiveBetween(0, 1000);
    }
}

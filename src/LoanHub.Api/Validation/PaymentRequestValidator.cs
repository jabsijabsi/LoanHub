using FluentValidation;
using LoanHub.Api.Contracts;

namespace LoanHub.Api.Validation;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public PaymentRequestValidator()
    {
        RuleFor(x => x.LoanId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Payment amount must be greater than 0.");
    }
}

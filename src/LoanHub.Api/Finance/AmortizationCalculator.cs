using LoanHub.Api.Entities;

namespace LoanHub.Api.Finance;

/// <summary>
/// Handles loan repayment math: the fixed monthly payment and the installment plan.
/// </summary>
public sealed class AmortizationCalculator
{
    /// <summary>
    /// Returns the level monthly payment for an amortizing loan.
    /// Uses PMT = P*i / (1 - (1+i)^-n); for a 0% rate it splits the principal evenly.
    /// </summary>
    public decimal MonthlyPayment(decimal principal, decimal annualRatePercent, int months)
    {
        if (months <= 0)
        {
            throw new ArgumentException("Term in months must be positive.", nameof(months));
        }

        var i = (double)annualRatePercent / 1200d; // monthly rate as a fraction

        if (i <= 0d)
        {
            return Round(principal / months);
        }

        var factor = Math.Pow(1d + i, months);
        var payment = (double)principal * i * factor / (factor - 1d);
        return Round((decimal)payment);
    }

    /// <summary>Builds the list of installments, putting any rounding remainder on the last one.</summary>
    public List<LoanSchedule> BuildPlan(decimal monthlyPayment, int months, DateTime firstDueFrom)
    {
        var plan = new List<LoanSchedule>();
        var planned = Round(monthlyPayment * months);
        var running = 0m;

        for (var seq = 1; seq <= months; seq++)
        {
            var amount = seq == months ? Round(planned - running) : monthlyPayment;
            running += amount;

            plan.Add(new LoanSchedule
            {
                Sequence = seq,
                PMT = amount,
                Date = firstDueFrom.AddMonths(seq)
            });
        }

        return plan;
    }

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}

using FluentAssertions;
using LoanHub.Api.Finance;
using Xunit;

namespace LoanHub.Tests;

public class AmortizationCalculatorTests
{
    private readonly AmortizationCalculator _calc = new();

    [Fact]
    public void MonthlyPayment_WithInterest_MatchesAmortizationFormula()
    {
        var pmt = _calc.MonthlyPayment(10_000m, 12m, 12);
        pmt.Should().BeApproximately(888.49m, 0.05m);
    }

    [Fact]
    public void MonthlyPayment_ZeroInterest_SplitsEvenly()
    {
        _calc.MonthlyPayment(1200m, 0m, 12).Should().Be(100m);
    }

    [Fact]
    public void BuildPlan_HasOnePerMonth_AndReconciles()
    {
        var monthly = _calc.MonthlyPayment(5000m, 10m, 10);
        var plan = _calc.BuildPlan(monthly, 10, new DateTime(2025, 1, 1));

        plan.Should().HaveCount(10);
        plan.Sum(p => p.PMT).Should().Be(Math.Round(monthly * 10, 2));
        plan[0].Sequence.Should().Be(1);
    }

    [Fact]
    public void MonthlyPayment_NonPositiveTerm_Throws()
    {
        var act = () => _calc.MonthlyPayment(1000m, 5m, 0);
        act.Should().Throw<ArgumentException>();
    }
}

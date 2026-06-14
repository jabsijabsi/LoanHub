using AutoMapper;
using LoanHub.Api.Contracts;
using LoanHub.Api.Entities;

namespace LoanHub.Api.Mapping;

/// <summary>Maps a loan to its view and fills in the computed payment totals.</summary>
public static class LoanViewFactory
{
    public static LoanView Build(IMapper mapper, Loan loan, decimal totalPaid)
    {
        var view = mapper.Map<LoanView>(loan);
        var payable = Math.Round(loan.MonthlyPayment * loan.TermMonths, 2, MidpointRounding.AwayFromZero);

        view.TotalPaid = totalPaid;
        view.Remaining = Math.Max(0m, payable - totalPaid);
        return view;
    }
}

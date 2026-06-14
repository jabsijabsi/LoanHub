using AutoMapper;
using LoanHub.Api.Common;
using LoanHub.Api.Contracts;
using LoanHub.Api.Data;
using LoanHub.Api.Entities;
using LoanHub.Api.Finance;
using LoanHub.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace LoanHub.Api.Services;

public class LoanService : ILoanService
{
    private const int MinAge = 18;
    private const int CreditFloor = 300;

    private readonly LoanHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IClock _clock;
    private readonly AmortizationCalculator _calculator;

    public LoanService(LoanHubDbContext db, IMapper mapper, IClock clock, AmortizationCalculator calculator)
    {
        _db = db;
        _mapper = mapper;
        _clock = clock;
        _calculator = calculator;
    }

    public async Task<LoanView> ApplyAsync(LoanApplicationRequest request, CancellationToken ct = default)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId, ct)
                       ?? throw new NotFoundException($"Customer {request.CustomerId} was not found.");

        var today = DateOnly.FromDateTime(_clock.UtcNow);
        if (DateHelpers.AgeInYears(customer.BirthDate, today) < MinAge)
        {
            throw new ConflictException($"Customer must be at least {MinAge} years old to borrow.");
        }

        var monthly = _calculator.MonthlyPayment(request.Amount, request.InterestRate, request.TermMonths);
        var approved = customer.CreditScore >= CreditFloor;

        var loan = new Loan
        {
            CustomerId = customer.Id,
            Amount = request.Amount,
            InterestRate = request.InterestRate,
            TermMonths = request.TermMonths,
            MonthlyPayment = monthly,
            Status = approved ? LoanState.Approved : LoanState.Rejected
        };

        if (approved)
        {
            loan.ScheduleItems = _calculator.BuildPlan(monthly, request.TermMonths, _clock.UtcNow);
        }

        _db.Loans.Add(loan);
        await _db.SaveChangesAsync(ct);

        return LoanViewFactory.Build(_mapper, loan, totalPaid: 0m);
    }

    public async Task<LoanView> GetAsync(int id, CancellationToken ct = default)
    {
        var loan = await _db.Loans
            .Include(l => l.Payments)
            .Include(l => l.ScheduleItems)
            .FirstOrDefaultAsync(l => l.Id == id, ct)
            ?? throw new NotFoundException($"Loan {id} was not found.");

        return LoanViewFactory.Build(_mapper, loan, loan.Payments.Sum(p => p.Amount));
    }
}

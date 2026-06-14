using LoanHub.Api.Common;
using LoanHub.Api.Contracts;
using LoanHub.Api.Data;
using LoanHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanHub.Api.Services;

public class PaymentService : IPaymentService
{
    private readonly LoanHubDbContext _db;
    private readonly IClock _clock;

    public PaymentService(LoanHubDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<PaymentReceipt> PayAsync(PaymentRequest request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
        {
            throw new ConflictException("Payment amount must be greater than 0.");
        }

        var loan = await _db.Loans
            .Include(l => l.Payments)
            .Include(l => l.ScheduleItems)
            .FirstOrDefaultAsync(l => l.Id == request.LoanId, ct)
            ?? throw new NotFoundException($"Loan {request.LoanId} was not found.");

        if (loan.Status is LoanState.Closed or LoanState.Rejected or LoanState.Pending)
        {
            throw new ConflictException($"Payments are not allowed on a {loan.Status} loan.");
        }

        var when = request.PaymentDate ?? _clock.UtcNow;

        var payment = new Payment
        {
            LoanId = loan.Id,
            Amount = request.Amount,
            PaymentDate = when
        };
        _db.Payments.Add(payment);

        var totalPaid = loan.Payments.Sum(p => p.Amount) + request.Amount;
        loan.Status = ResolveState(loan, totalPaid, _clock.UtcNow);

        await _db.SaveChangesAsync(ct);

        var payable = Math.Round(loan.MonthlyPayment * loan.TermMonths, 2, MidpointRounding.AwayFromZero);

        return new PaymentReceipt(
            payment.Id,
            loan.Id,
            payment.Amount,
            payment.PaymentDate,
            totalPaid,
            Math.Max(0m, payable - totalPaid),
            loan.Status.ToString());
    }

    private static LoanState ResolveState(Loan loan, decimal totalPaid, DateTime now)
    {
        var payable = Math.Round(loan.MonthlyPayment * loan.TermMonths, 2, MidpointRounding.AwayFromZero);
        if (totalPaid >= payable)
        {
            return LoanState.Closed;
        }

        var dueSoFar = loan.ScheduleItems
            .Where(s => s.Date.Date <= now.Date)
            .Sum(s => s.PMT);

        return totalPaid < dueSoFar ? LoanState.Overdue : LoanState.Approved;
    }
}

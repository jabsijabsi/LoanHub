using FluentAssertions;
using LoanHub.Api.Common;
using LoanHub.Api.Contracts;
using LoanHub.Api.Entities;
using LoanHub.Api.Services;
using Xunit;

namespace LoanHub.Tests;

public class PaymentServiceTests
{
    private static readonly DateTime Now = new(2025, 6, 1);

    private static PaymentService Build(Api.Data.LoanHubDbContext db)
        => new(db, new TestClock(Now));

    private static async Task<Loan> SeedLoan(
        Api.Data.LoanHubDbContext db,
        LoanState status,
        decimal monthly = 100m,
        int term = 12,
        int pastDue = 0)
    {
        var loan = new Loan
        {
            Customer = new Customer
            {
                FirstName = "L", LastName = "H",
                PersonalNumber = Guid.NewGuid().ToString("N")[..11],
                BirthDate = new DateOnly(1990, 1, 1), CreditScore = 700
            },
            Amount = monthly * term,
            InterestRate = 0m,
            TermMonths = term,
            MonthlyPayment = monthly,
            Status = status
        };

        for (var i = 1; i <= term; i++)
        {
            var date = i <= pastDue ? Now.AddMonths(-(pastDue - i + 1)) : Now.AddMonths(i);
            loan.ScheduleItems.Add(new LoanSchedule { Sequence = i, PMT = monthly, Date = date });
        }

        db.Loans.Add(loan);
        await db.SaveChangesAsync();
        return loan;
    }

    [Fact]
    public async Task Pay_ClosedLoan_Conflict()
    {
        using var db = TestKit.NewDb();
        var loan = await SeedLoan(db, LoanState.Closed);
        var sut = Build(db);

        var act = () => sut.PayAsync(new PaymentRequest(loan.Id, 50m, null));

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Pay_FullAmount_Closes()
    {
        using var db = TestKit.NewDb();
        var loan = await SeedLoan(db, LoanState.Approved, 100m, 12);
        var sut = Build(db);

        var receipt = await sut.PayAsync(new PaymentRequest(loan.Id, 1200m, null));

        receipt.Status.Should().Be(nameof(LoanState.Closed));
        receipt.Remaining.Should().Be(0m);
    }

    [Fact]
    public async Task Pay_PartialNothingDueYet_StaysApproved()
    {
        using var db = TestKit.NewDb();
        var loan = await SeedLoan(db, LoanState.Approved, 100m, 12);
        var sut = Build(db);

        var receipt = await sut.PayAsync(new PaymentRequest(loan.Id, 100m, null));

        receipt.Status.Should().Be(nameof(LoanState.Approved));
        receipt.Remaining.Should().Be(1100m);
    }

    [Fact]
    public async Task Pay_BehindSchedule_Overdue()
    {
        using var db = TestKit.NewDb();
        var loan = await SeedLoan(db, LoanState.Approved, 100m, 12, pastDue: 3);
        var sut = Build(db);

        var receipt = await sut.PayAsync(new PaymentRequest(loan.Id, 100m, null));

        receipt.Status.Should().Be(nameof(LoanState.Overdue));
    }

    [Fact]
    public async Task Pay_MissingLoan_NotFound()
    {
        using var db = TestKit.NewDb();
        var sut = Build(db);

        var act = () => sut.PayAsync(new PaymentRequest(4242, 100m, null));

        await act.Should().ThrowAsync<NotFoundException>();
    }
}

using FluentAssertions;
using LoanHub.Api.Common;
using LoanHub.Api.Contracts;
using LoanHub.Api.Entities;
using LoanHub.Api.Finance;
using LoanHub.Api.Services;
using Xunit;

namespace LoanHub.Tests;

public class LoanServiceTests
{
    private static readonly DateTime Now = new(2025, 6, 1);

    private static LoanService Build(Api.Data.LoanHubDbContext db)
        => new(db, TestKit.NewMapper(), new TestClock(Now), new AmortizationCalculator());

    private static async Task<Customer> SeedCustomer(Api.Data.LoanHubDbContext db, int credit, DateOnly? birth = null)
    {
        var c = new Customer
        {
            FirstName = "T", LastName = "C",
            PersonalNumber = Guid.NewGuid().ToString("N")[..11],
            BirthDate = birth ?? new DateOnly(1990, 1, 1),
            CreditScore = credit
        };
        db.Customers.Add(c);
        await db.SaveChangesAsync();
        return c;
    }

    [Fact]
    public async Task Apply_GoodCredit_ApprovedWithSchedule()
    {
        using var db = TestKit.NewDb();
        var customer = await SeedCustomer(db, 500);
        var sut = Build(db);

        var view = await sut.ApplyAsync(new LoanApplicationRequest(customer.Id, 10_000m, 12m, 12));

        view.Status.Should().Be(nameof(LoanState.Approved));
        view.Schedule.Should().HaveCount(12);
        view.MonthlyPayment.Should().BeGreaterThan(0m);
    }

    [Fact]
    public async Task Apply_LowCredit_RejectedNoSchedule()
    {
        using var db = TestKit.NewDb();
        var customer = await SeedCustomer(db, 250);
        var sut = Build(db);

        var view = await sut.ApplyAsync(new LoanApplicationRequest(customer.Id, 5_000m, 10m, 12));

        view.Status.Should().Be(nameof(LoanState.Rejected));
        view.Schedule.Should().BeEmpty();
    }

    [Fact]
    public async Task Apply_MissingCustomer_NotFound()
    {
        using var db = TestKit.NewDb();
        var sut = Build(db);

        var act = () => sut.ApplyAsync(new LoanApplicationRequest(999, 5_000m, 10m, 12));

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Apply_Under18_Conflict()
    {
        using var db = TestKit.NewDb();
        var customer = await SeedCustomer(db, 700, new DateOnly(2010, 1, 1));
        var sut = Build(db);

        var act = () => sut.ApplyAsync(new LoanApplicationRequest(customer.Id, 5_000m, 10m, 12));

        await act.Should().ThrowAsync<ConflictException>();
    }
}

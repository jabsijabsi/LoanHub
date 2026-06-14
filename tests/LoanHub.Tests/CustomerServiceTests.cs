using FluentAssertions;
using LoanHub.Api.Common;
using LoanHub.Api.Contracts;
using LoanHub.Api.Entities;
using LoanHub.Api.Services;
using Xunit;

namespace LoanHub.Tests;

public class CustomerServiceTests
{
    private static readonly DateTime Now = new(2025, 6, 1);

    private static CustomerService Build(Api.Data.LoanHubDbContext db)
        => new(db, TestKit.NewMapper(), new TestClock(Now));

    [Fact]
    public async Task CreateAsync_Valid_Persists()
    {
        using var db = TestKit.NewDb();
        var sut = Build(db);

        var view = await sut.CreateAsync(new NewCustomerRequest("Ana", "Ng", "10101010101", new DateOnly(1990, 1, 1), 700));

        view.Id.Should().BeGreaterThan(0);
        db.Customers.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateAsync_Under18_Throws()
    {
        using var db = TestKit.NewDb();
        var sut = Build(db);

        var act = () => sut.CreateAsync(new NewCustomerRequest("Kid", "Young", "20202020202", new DateOnly(2010, 1, 1), 700));

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task CreateAsync_DuplicatePersonalNumber_Throws()
    {
        using var db = TestKit.NewDb();
        db.Customers.Add(new Customer
        {
            FirstName = "X", LastName = "Y", PersonalNumber = "33333333333",
            BirthDate = new DateOnly(1985, 1, 1), CreditScore = 600
        });
        await db.SaveChangesAsync();

        var sut = Build(db);
        var act = () => sut.CreateAsync(new NewCustomerRequest("New", "Guy", "33333333333", new DateOnly(1991, 1, 1), 650));

        await act.Should().ThrowAsync<ConflictException>();
    }
}

using LoanHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanHub.Api.Data;

/// <summary>Creates the database on startup and inserts a few sample customers.</summary>
public static class DatabaseBootstrapper
{
    public static async Task PrepareAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LoanHubDbContext>();

        await db.Database.EnsureCreatedAsync();

        if (await db.Customers.AnyAsync())
        {
            return;
        }

        db.Customers.AddRange(
            new Customer
            {
                FirstName = "Luka",
                LastName = "Maisuradze",
                PersonalNumber = "11223344556",
                BirthDate = new DateOnly(1992, 7, 14),
                CreditScore = 710
            },
            new Customer
            {
                FirstName = "Salome",
                LastName = "Dvali",
                PersonalNumber = "66778899001",
                BirthDate = new DateOnly(2001, 3, 9),
                CreditScore = 240
            });

        await db.SaveChangesAsync();
    }
}

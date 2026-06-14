using System.Linq.Expressions;
using LoanHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanHub.Api.Data;

public class LoanHubDbContext : DbContext
{
    public LoanHubDbContext(DbContextOptions<LoanHubDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<LoanSchedule> LoanSchedules => Set<LoanSchedule>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Customer>(e =>
        {
            e.Property(c => c.FirstName).IsRequired().HasMaxLength(80);
            e.Property(c => c.LastName).IsRequired().HasMaxLength(80);
            e.Property(c => c.PersonalNumber).IsRequired().HasMaxLength(40);
            e.HasIndex(c => c.PersonalNumber).IsUnique();
            e.HasMany(c => c.Loans).WithOne(l => l.Customer!).HasForeignKey(l => l.CustomerId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Loan>(e =>
        {
            e.Property(l => l.Amount).HasPrecision(18, 2);
            e.Property(l => l.InterestRate).HasPrecision(9, 4);
            e.Property(l => l.MonthlyPayment).HasPrecision(18, 2);
            e.Property(l => l.Status).HasConversion<string>().HasMaxLength(16);
            e.HasMany(l => l.Payments).WithOne(p => p.Loan!).HasForeignKey(p => p.LoanId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(l => l.ScheduleItems).WithOne(s => s.Loan!).HasForeignKey(s => s.LoanId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Payment>(e => e.Property(p => p.Amount).HasPrecision(18, 2));
        builder.Entity<LoanSchedule>(e => e.Property(s => s.PMT).HasPrecision(18, 2));

        // Hide soft-deleted rows from every query automatically.
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entity.ClrType))
            {
                var param = Expression.Parameter(entity.ClrType, "x");
                var body = Expression.Equal(
                    Expression.Property(param, nameof(ISoftDeletable.IsDeleted)),
                    Expression.Constant(false));
                builder.Entity(entity.ClrType).HasQueryFilter(Expression.Lambda(body, param));
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Stamp();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        Stamp();
        return base.SaveChanges();
    }

    private void Stamp()
    {
        var timestamp = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedOn = timestamp;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = timestamp;
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.ModifiedOn = timestamp;
            }
        }
    }
}

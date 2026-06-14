using AutoMapper;
using LoanHub.Api.Common;
using LoanHub.Api.Contracts;
using LoanHub.Api.Data;
using LoanHub.Api.Entities;
using LoanHub.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace LoanHub.Api.Services;

public class CustomerService : ICustomerService
{
    private const int MinAge = 18;

    private readonly LoanHubDbContext _db;
    private readonly IMapper _mapper;
    private readonly IClock _clock;

    public CustomerService(LoanHubDbContext db, IMapper mapper, IClock clock)
    {
        _db = db;
        _mapper = mapper;
        _clock = clock;
    }

    public async Task<CustomerView> CreateAsync(NewCustomerRequest request, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(_clock.UtcNow);
        if (DateHelpers.AgeInYears(request.BirthDate, today) < MinAge)
        {
            throw new ConflictException($"Customer must be at least {MinAge} years old.");
        }

        var personalNumber = request.PersonalNumber.Trim();
        var taken = await _db.Customers.AnyAsync(c => c.PersonalNumber == personalNumber, ct);
        if (taken)
        {
            throw new ConflictException($"Personal number '{personalNumber}' is already registered.");
        }

        var customer = new Customer
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PersonalNumber = personalNumber,
            BirthDate = request.BirthDate,
            CreditScore = request.CreditScore
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<CustomerView>(customer);
    }

    public async Task<CustomerView> GetAsync(int id, CancellationToken ct = default)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct)
                       ?? throw new NotFoundException($"Customer {id} was not found.");

        return _mapper.Map<CustomerView>(customer);
    }

    public async Task<IReadOnlyList<LoanView>> GetLoanHistoryAsync(int customerId, CancellationToken ct = default)
    {
        var exists = await _db.Customers.AnyAsync(c => c.Id == customerId, ct);
        if (!exists)
        {
            throw new NotFoundException($"Customer {customerId} was not found.");
        }

        var loans = await _db.Loans
            .Include(l => l.Payments)
            .Include(l => l.ScheduleItems)
            .Where(l => l.CustomerId == customerId)
            .OrderByDescending(l => l.CreatedOn)
            .ToListAsync(ct);

        return loans
            .Select(l => LoanViewFactory.Build(_mapper, l, l.Payments.Sum(p => p.Amount)))
            .ToList();
    }
}

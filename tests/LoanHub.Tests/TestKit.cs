using AutoMapper;
using LoanHub.Api.Common;
using LoanHub.Api.Data;
using LoanHub.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace LoanHub.Tests;

public sealed class TestClock : IClock
{
    public TestClock(DateTime utcNow) => UtcNow = utcNow;
    public DateTime UtcNow { get; set; }
}

public static class TestKit
{
    public static LoanHubDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<LoanHubDbContext>()
            .UseInMemoryDatabase($"loanhub-{Guid.NewGuid()}")
            .Options;
        return new LoanHubDbContext(options);
    }

    public static IMapper NewMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }
}

using AutoMapper;
using LoanHub.Api.Contracts;
using LoanHub.Api.Entities;

namespace LoanHub.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Customer, CustomerView>();

        CreateMap<LoanSchedule, ScheduleItemView>();

        CreateMap<Loan, LoanView>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Schedule, o => o.MapFrom(s => s.ScheduleItems.OrderBy(x => x.Sequence)))
            // Computed in the service after the totals are known.
            .ForMember(d => d.TotalPaid, o => o.Ignore())
            .ForMember(d => d.Remaining, o => o.Ignore());
    }
}

using AutoMapper;
using feasibility.Entity.Dtos.ActivityLog;
using feasibility.Entity.Entities.Logging;

namespace feasibility.Business.Mappings;

public class ActivityLogProfile : Profile
{
    public ActivityLogProfile()
    {
        CreateMap<ActivityLog, ActivityLogListDto>();
        CreateMap<ActivityLog, ActivityLogDetailDto>();
    }
}

using AutoMapper;
using feasibility.Entity.Dtos.DeviceLine;
using feasibility.Entity.Entities.FeasibilityAmortization;

namespace feasibility.Business.Mappings;

public class DeviceLineProfile : Profile
{
    public DeviceLineProfile()
    {
        CreateMap<DeviceLineCreateDto, DeviceLine>();

        CreateMap<DeviceLineUpdateDto, DeviceLine>()
            .ForMember(d => d.Id, c => c.MapFrom(s => s.Id));

        CreateMap<DeviceLine, DeviceLineUpdateDto>();

        CreateMap<DeviceLine, DeviceLineListDto>()
            .ForMember(d => d.StudyName,
                c => c.MapFrom(s => s.Study != null ? s.Study.FeasibilityName : string.Empty));

        CreateMap<DeviceLine, DeviceLineDetailDto>()
            .ForMember(d => d.StudyName,
                c => c.MapFrom(s => s.Study != null ? s.Study.FeasibilityName : string.Empty))
            .ForMember(d => d.Audit, c => c.MapFrom(s => s));
    }
}

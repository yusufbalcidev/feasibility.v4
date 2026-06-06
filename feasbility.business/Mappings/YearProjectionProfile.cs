using AutoMapper;
using feasibility.Entity.Dtos.YearProjection;
using feasibility.Entity.Entities.FeasibilityAmortization;

namespace feasibility.Business.Mappings;

public class YearProjectionProfile : Profile
{
    public YearProjectionProfile()
    {
        CreateMap<YearProjectionCreateDto, YearProjection>();

        CreateMap<YearProjectionUpdateDto, YearProjection>()
            .ForMember(d => d.Id, c => c.MapFrom(s => s.Id));

        CreateMap<YearProjection, YearProjectionUpdateDto>();

        CreateMap<YearProjection, YearProjectionListDto>();

        CreateMap<YearProjection, YearProjectionDetailDto>()
            .ForMember(d => d.Audit, c => c.MapFrom(s => s));
    }
}

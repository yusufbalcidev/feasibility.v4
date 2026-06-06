using AutoMapper;
using feasibility.Entity.Dtos.Study;
using feasibility.Entity.Entities.FeasibilityAmortization;

namespace feasibility.Business.Mappings;

public class StudyProfile : Profile
{
    public StudyProfile()
    {
        CreateMap<StudyCreateDto, Study>();

        CreateMap<StudyUpdateDto, Study>()
            .ForMember(d => d.Id, c => c.MapFrom(s => s.Id));

        CreateMap<Study, StudyUpdateDto>();

        CreateMap<Study, StudyListDto>()
            .ForMember(d => d.LocationName,
                c => c.MapFrom(s => s.Location != null ? s.Location.Name : string.Empty));

        CreateMap<Study, StudyDetailDto>()
            .ForMember(d => d.LocationName,
                c => c.MapFrom(s => s.Location != null ? s.Location.Name : string.Empty))
            .ForMember(d => d.Audit, c => c.MapFrom(s => s));
    }
}

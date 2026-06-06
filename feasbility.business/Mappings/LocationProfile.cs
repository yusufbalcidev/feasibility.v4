using AutoMapper;
using feasibility.Entity.Dtos.Location;
using feasibility.Entity.Entities.Locations;

namespace feasibility.Business.Mappings;

public class LocationProfile : Profile
{
    public LocationProfile()
    {
        CreateMap<LocationCreateDto, Location>();

        CreateMap<LocationUpdateDto, Location>()
            .ForMember(d => d.Id, c => c.MapFrom(s => s.Id));

        CreateMap<Location, LocationUpdateDto>();

        CreateMap<Location, LocationListDto>()
            .ForMember(d => d.LocationTypeMaintenanceName,
                c => c.MapFrom(s => s.LocationTypeMaintenance != null ? s.LocationTypeMaintenance.Name : null));

        CreateMap<Location, LocationDetailDto>()
            .ForMember(d => d.LocationTypeMaintenanceName,
                c => c.MapFrom(s => s.LocationTypeMaintenance != null ? s.LocationTypeMaintenance.Name : null))
            .ForMember(d => d.Audit, c => c.MapFrom(s => s));
    }
}

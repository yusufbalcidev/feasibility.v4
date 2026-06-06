using AutoMapper;
using feasibility.Entity.Dtos.LocationTypeMaintenance;
using feasibility.Entity.Entities.Locations;

namespace feasibility.Business.Mappings;

public class LocationTypeMaintenanceProfile : Profile
{
    public LocationTypeMaintenanceProfile()
    {
        CreateMap<LocationTypeMaintenanceCreateDto, LocationTypeMaintenance>();

        CreateMap<LocationTypeMaintenanceUpdateDto, LocationTypeMaintenance>()
            .ForMember(d => d.Id, c => c.MapFrom(s => s.Id));

        CreateMap<LocationTypeMaintenance, LocationTypeMaintenanceUpdateDto>();

        CreateMap<LocationTypeMaintenance, LocationTypeMaintenanceListDto>()
            .ForMember(d => d.LocationCount, c => c.MapFrom(s => s.Locations.Count(l => !l.IsDeleted)));

        CreateMap<LocationTypeMaintenance, LocationTypeMaintenanceDetailDto>()
            .ForMember(d => d.LocationCount, c => c.MapFrom(s => s.Locations.Count(l => !l.IsDeleted)))
            .ForMember(d => d.Audit, c => c.MapFrom(s => s));
    }
}

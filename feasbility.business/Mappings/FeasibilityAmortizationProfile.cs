using AutoMapper;
using feasibility.Entity.Dtos.FeasibilityAmortization;
using feasibility.Entity.Entities.FeasibilityAmortization;

namespace feasibility.Business.Mappings;

public class FeasibilityAmortizationProfile : Profile
{
    public FeasibilityAmortizationProfile()
    {
        CreateMap<FeasibilityAmortizationSaveDto, Study>()
            .ForMember(d => d.Id,                            o => o.Ignore())
            .ForMember(d => d.Version,                       o => o.Ignore())
            .ForMember(d => d.MonthlyRentTl,                 o => o.Ignore())
            .ForMember(d => d.PostWarrantyMaintenanceCostTl, o => o.Ignore())
            .ForMember(d => d.AdvertisingRevenueTl,          o => o.Ignore())
            .ForMember(d => d.StationUnitCostTl,             o => o.Ignore())
            .ForMember(d => d.ProviderEntryFeeTl,            o => o.Ignore())
            .ForMember(d => d.InfrastructureCostTl,          o => o.Ignore())
            .ForMember(d => d.DeviceUnitCostTl,              o => o.Ignore())
            .ForMember(d => d.Location,                      o => o.Ignore())
            .ForMember(d => d.DeviceLines,                   o => o.Ignore())
            .ForMember(d => d.CreatedAt,                     o => o.Ignore())
            .ForMember(d => d.CreatedBy,                     o => o.Ignore())
            .ForMember(d => d.CreatedByName,                 o => o.Ignore())
            .ForMember(d => d.UpdatedAt,                     o => o.Ignore())
            .ForMember(d => d.UpdatedBy,                     o => o.Ignore())
            .ForMember(d => d.UpdatedByName,                 o => o.Ignore())
            .ForMember(d => d.IsDeleted,                     o => o.Ignore())
            .ForMember(d => d.DeletedAt,                     o => o.Ignore())
            .ForMember(d => d.DeletedBy,                     o => o.Ignore())
            .ForMember(d => d.DeletedByName,                 o => o.Ignore());

        CreateMap<DeviceLineSaveDto, DeviceLine>()
            .ForMember(d => d.Id,                    o => o.Ignore())
            .ForMember(d => d.StudyId,               o => o.Ignore())
            .ForMember(d => d.Study,                 o => o.Ignore())
            .ForMember(d => d.YearProjections,       o => o.Ignore())
            .ForMember(d => d.UnitLocationCostTl,    o => o.Ignore())
            .ForMember(d => d.CreatedAt,             o => o.Ignore())
            .ForMember(d => d.CreatedBy,             o => o.Ignore())
            .ForMember(d => d.CreatedByName,         o => o.Ignore())
            .ForMember(d => d.UpdatedAt,             o => o.Ignore())
            .ForMember(d => d.UpdatedBy,             o => o.Ignore())
            .ForMember(d => d.UpdatedByName,         o => o.Ignore())
            .ForMember(d => d.IsDeleted,             o => o.Ignore())
            .ForMember(d => d.DeletedAt,             o => o.Ignore())
            .ForMember(d => d.DeletedBy,             o => o.Ignore())
            .ForMember(d => d.DeletedByName,         o => o.Ignore());

        CreateMap<YearProjectionSaveDto, YearProjection>()
            .ForMember(d => d.Id,             o => o.Ignore())
            .ForMember(d => d.DeviceLineId,   o => o.Ignore())
            .ForMember(d => d.DeviceLine,     o => o.Ignore())
            .ForMember(d => d.CreatedAt,      o => o.Ignore())
            .ForMember(d => d.CreatedBy,      o => o.Ignore())
            .ForMember(d => d.CreatedByName,  o => o.Ignore())
            .ForMember(d => d.UpdatedAt,      o => o.Ignore())
            .ForMember(d => d.UpdatedBy,      o => o.Ignore())
            .ForMember(d => d.UpdatedByName,  o => o.Ignore())
            .ForMember(d => d.IsDeleted,      o => o.Ignore())
            .ForMember(d => d.DeletedAt,      o => o.Ignore())
            .ForMember(d => d.DeletedBy,      o => o.Ignore())
            .ForMember(d => d.DeletedByName,  o => o.Ignore());
    }
}

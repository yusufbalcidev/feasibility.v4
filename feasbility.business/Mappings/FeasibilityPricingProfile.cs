using AutoMapper;
using feasibility.Entity.Dtos.FeasibilityPricing;
using feasibility.Entity.Entities.FeasibilityPricing;

namespace feasibility.Business.Mappings;

public class FeasibilityPricingProfile : Profile
{
    public FeasibilityPricingProfile()
    {
        CreateMap<FeasibilityPricingSaveDto, PricingStudy>()
            .ForMember(d => d.Id,            o => o.Ignore())
            .ForMember(d => d.Version,       o => o.Ignore())
            .ForMember(d => d.Stations,      o => o.Ignore())
            .ForMember(d => d.CreatedAt,     o => o.Ignore())
            .ForMember(d => d.CreatedBy,     o => o.Ignore())
            .ForMember(d => d.CreatedByName, o => o.Ignore())
            .ForMember(d => d.UpdatedAt,     o => o.Ignore())
            .ForMember(d => d.UpdatedBy,     o => o.Ignore())
            .ForMember(d => d.UpdatedByName, o => o.Ignore())
            .ForMember(d => d.IsDeleted,     o => o.Ignore())
            .ForMember(d => d.DeletedAt,     o => o.Ignore())
            .ForMember(d => d.DeletedBy,     o => o.Ignore())
            .ForMember(d => d.DeletedByName, o => o.Ignore());

        CreateMap<PricingStationSaveDto, PricingStation>()
            .ForMember(d => d.Id,                   o => o.Ignore())
            .ForMember(d => d.PricingStudyId,       o => o.Ignore())
            .ForMember(d => d.PricingStudy,         o => o.Ignore())
            .ForMember(d => d.HardwareCostTl,       o => o.Ignore())
            .ForMember(d => d.InfrastructureCostTl, o => o.Ignore())
            .ForMember(d => d.AnnualOpexUsd,        o => o.Ignore())
            .ForMember(d => d.CreatedAt,            o => o.Ignore())
            .ForMember(d => d.CreatedBy,            o => o.Ignore())
            .ForMember(d => d.CreatedByName,        o => o.Ignore())
            .ForMember(d => d.UpdatedAt,            o => o.Ignore())
            .ForMember(d => d.UpdatedBy,            o => o.Ignore())
            .ForMember(d => d.UpdatedByName,        o => o.Ignore())
            .ForMember(d => d.IsDeleted,            o => o.Ignore())
            .ForMember(d => d.DeletedAt,            o => o.Ignore())
            .ForMember(d => d.DeletedBy,            o => o.Ignore())
            .ForMember(d => d.DeletedByName,        o => o.Ignore());
    }
}

using AutoMapper;
using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Entities.Common;
using feasibility.Entity.Entities.Identity;

namespace feasibility.Business.Mappings;

public class AuditProfile : Profile
{
    public AuditProfile()
    {
        CreateMap<BaseEntity, AuditInfoDto>();

        CreateMap<AppUser, AuditInfoDto>()
            .ForMember(d => d.CreatedAt, c => c.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.CreatedByName, c => c.MapFrom(s => s.CreatedByName))
            .ForMember(d => d.UpdatedAt, c => c.MapFrom(s => s.UpdatedAt))
            .ForMember(d => d.UpdatedByName, c => c.MapFrom(s => s.UpdatedByName))
            .ForMember(d => d.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
            .ForMember(d => d.DeletedAt, c => c.MapFrom(s => s.DeletedAt))
            .ForMember(d => d.DeletedByName, c => c.MapFrom(s => s.DeletedByName));

        CreateMap<AppRole, AuditInfoDto>()
            .ForMember(d => d.CreatedAt, c => c.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.CreatedByName, c => c.MapFrom(s => s.CreatedByName))
            .ForMember(d => d.UpdatedAt, c => c.MapFrom(s => s.UpdatedAt))
            .ForMember(d => d.UpdatedByName, c => c.MapFrom(s => s.UpdatedByName))
            .ForMember(d => d.IsDeleted, c => c.MapFrom(s => s.IsDeleted))
            .ForMember(d => d.DeletedAt, c => c.MapFrom(s => s.DeletedAt))
            .ForMember(d => d.DeletedByName, c => c.MapFrom(s => s.DeletedByName));
    }
}

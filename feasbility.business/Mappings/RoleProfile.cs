using AutoMapper;
using feasibility.Entity.Dtos.Role;
using feasibility.Entity.Entities.Authorization;
using feasibility.Entity.Entities.Identity;

namespace feasibility.Business.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<RoleCreateDto, AppRole>();

        CreateMap<RoleUpdateDto, AppRole>()
            .ForMember(d => d.Id, c => c.MapFrom(s => s.Id));

        CreateMap<AppRole, RoleListDto>()
            .ForMember(d => d.UserCount, c => c.Ignore())
            .ForMember(d => d.IsSystem, c => c.Ignore());

        CreateMap<AppRole, RoleDetailDto>()
            .ForMember(d => d.UserCount, c => c.Ignore())
            .ForMember(d => d.IsSystem, c => c.Ignore())
            .ForMember(d => d.Permissions, c => c.Ignore())
            .ForMember(d => d.Audit, c => c.MapFrom(s => s));

        CreateMap<AppRole, RoleUpdateDto>()
            .ForMember(d => d.Permissions, c => c.Ignore());

        CreateMap<RolePermission, RolePagePermissionDto>()
            .ForMember(d => d.PageKey, c => c.MapFrom(s => s.Page!.Key))
            .ForMember(d => d.PageName, c => c.MapFrom(s => s.Page!.Name));
    }
}

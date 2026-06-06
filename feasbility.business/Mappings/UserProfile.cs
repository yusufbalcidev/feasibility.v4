using AutoMapper;
using feasibility.Entity.Dtos.User;
using feasibility.Entity.Entities.Identity;

namespace feasibility.Business.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserCreateDto, AppUser>()
            .ForMember(d => d.EmailConfirmed, c => c.MapFrom(_ => true));

        CreateMap<UserUpdateDto, AppUser>()
            .ForMember(d => d.Id, c => c.MapFrom(s => s.Id));

        CreateMap<AppUser, UserListDto>()
            .ForMember(d => d.RoleName, c => c.Ignore())
            .ForMember(d => d.RoleId, c => c.Ignore());

        CreateMap<AppUser, UserDetailDto>()
            .ForMember(d => d.RoleName, c => c.Ignore())
            .ForMember(d => d.RoleId, c => c.Ignore())
            .ForMember(d => d.Audit, c => c.MapFrom(s => s));

        CreateMap<AppUser, UserUpdateDto>()
            .ForMember(d => d.NewPassword, c => c.Ignore())
            .ForMember(d => d.RoleId, c => c.Ignore());
    }
}

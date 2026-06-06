using feasibility.Entity.Dtos.Role;

namespace feasibility.Business.Abstract;

public interface IRolePermissionService
{
    Task<List<RolePagePermissionDto>> GetPermissionsForRoleAsync(Guid roleId, CancellationToken ct = default);
    Task<List<RolePagePermissionDto>> GetPermissionTemplateAsync(CancellationToken ct = default);
    Task SaveRolePermissionsAsync(Guid roleId, IEnumerable<RolePagePermissionDto> permissions, CancellationToken ct = default);
    Task<bool> UserHasPagePermissionAsync(Guid userId, string pageKey, string action, CancellationToken ct = default);
    Task<HashSet<string>> GetVisiblePagesForUserAsync(Guid userId, CancellationToken ct = default);
}

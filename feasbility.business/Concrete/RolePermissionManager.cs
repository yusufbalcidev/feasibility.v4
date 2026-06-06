using feasibility.Business.Abstract;
using feasibility.DataAccess.Context;
using feasibility.Entity.Dtos.Role;
using feasibility.Entity.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace feasibility.Business.Concrete;

public class RolePermissionManager : IRolePermissionService
{
    private readonly AppDbContext _context;

    public RolePermissionManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RolePagePermissionDto>> GetPermissionTemplateAsync(CancellationToken ct = default)
    {
        return await _context.Pages
            .OrderBy(p => p.DisplayOrder)
            .Select(p => new RolePagePermissionDto
            {
                PageId = p.Id,
                PageKey = p.Key,
                PageName = p.Name,
                CanView = false,
                CanCreate = false,
                CanEdit = false,
                CanDelete = false
            })
            .ToListAsync(ct);
    }

    public async Task<List<RolePagePermissionDto>> GetPermissionsForRoleAsync(Guid roleId, CancellationToken ct = default)
    {
        var template = await GetPermissionTemplateAsync(ct);
        var existing = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(ct);

        foreach (var dto in template)
        {
            var current = existing.FirstOrDefault(e => e.PageId == dto.PageId);
            if (current is null) continue;
            dto.CanView = current.CanView;
            dto.CanCreate = current.CanCreate;
            dto.CanEdit = current.CanEdit;
            dto.CanDelete = current.CanDelete;
        }
        return template;
    }

    public async Task SaveRolePermissionsAsync(Guid roleId, IEnumerable<RolePagePermissionDto> permissions, CancellationToken ct = default)
    {
        var existing = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(ct);

        foreach (var dto in permissions)
        {
            var current = existing.FirstOrDefault(e => e.PageId == dto.PageId);
            if (current is null)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PageId = dto.PageId,
                    CanView = dto.CanView,
                    CanCreate = dto.CanCreate,
                    CanEdit = dto.CanEdit,
                    CanDelete = dto.CanDelete
                });
            }
            else
            {
                current.CanView = dto.CanView;
                current.CanCreate = dto.CanCreate;
                current.CanEdit = dto.CanEdit;
                current.CanDelete = dto.CanDelete;
                _context.RolePermissions.Update(current);
            }
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> UserHasPagePermissionAsync(Guid userId, string pageKey, string action, CancellationToken ct = default)
    {
        var roleIds = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);

        if (roleIds.Count == 0) return false;

        var query = _context.RolePermissions
            .Include(rp => rp.Page)
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.Page!.Key == pageKey);

        return action.ToLowerInvariant() switch
        {
            "view" => await query.AnyAsync(rp => rp.CanView, ct),
            "create" => await query.AnyAsync(rp => rp.CanCreate, ct),
            "edit" => await query.AnyAsync(rp => rp.CanEdit, ct),
            "delete" => await query.AnyAsync(rp => rp.CanDelete, ct),
            _ => false
        };
    }

    public async Task<HashSet<string>> GetVisiblePagesForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var roleIds = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);

        if (roleIds.Count == 0) return new HashSet<string>();

        var keys = await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.CanView)
            .Include(rp => rp.Page)
            .Select(rp => rp.Page!.Key)
            .ToListAsync(ct);

        return keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}

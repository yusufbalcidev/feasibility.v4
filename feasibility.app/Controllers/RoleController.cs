using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Context;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Dtos.Role;
using feasibility.Entity.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace feasibility.App.Controllers;

[PagePermission(PageSeed.RoleKey)]
public class RoleController : Controller
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly AppDbContext _context;
    private readonly IRolePermissionService _permissionService;

    public RoleController(RoleManager<AppRole> roleManager, AppDbContext context, IRolePermissionService permissionService)
    {
        _roleManager = roleManager;
        _context = context;
        _permissionService = permissionService;
    }

    private const int PageSize = 10;

    public async Task<IActionResult> Index(string? q, int page = 1, CancellationToken ct = default)
    {
        if (page < 1) page = 1;

        var query = _context.Roles.IgnoreQueryFilters().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(r =>
                (r.Name != null && r.Name.Contains(term)) ||
                (r.Description != null && r.Description.Contains(term)));
        }

        var totalCount = await query.CountAsync(ct);
        var roles = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(ct);

        var roleIds = roles.Select(r => r.Id).ToList();
        var counts = await _context.UserRoles
            .Where(ur => roleIds.Contains(ur.RoleId))
            .GroupBy(ur => ur.RoleId)
            .Select(g => new { RoleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoleId, x => x.Count, ct);

        var list = roles.Select(r => new RoleListDto
        {
            Id = r.Id,
            Name = r.Name ?? string.Empty,
            Description = r.Description,
            UserCount = counts.TryGetValue(r.Id, out var c) ? c : 0,
            IsDeleted = r.IsDeleted,
            CreatedAt = r.CreatedAt,
            CreatedByName = r.CreatedByName,
            IsSystem = r.Id == RoleSeed.SuperAdminRoleId
        }).ToList();

        var model = PagedResult<RoleListDto>.Create(list, totalCount, page, PageSize, q);
        return View(model);
    }

    [HttpGet]
    [PagePermission(PageSeed.RoleKey, "create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var dto = new RoleCreateDto
        {
            Permissions = await _permissionService.GetPermissionTemplateAsync(ct)
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.RoleKey, "create")]
    public async Task<IActionResult> Create(RoleCreateDto dto, CancellationToken ct)
    {
        await NormalizePermissionsAsync(dto.Permissions, ct);

        if (!ModelState.IsValid) return View(dto);

        var role = new AppRole
        {
            Name = dto.Name,
            Description = dto.Description
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(dto);
        }

        await _permissionService.SaveRolePermissionsAsync(role.Id, dto.Permissions, ct);
        TempData["Success"] = "Rol başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PagePermission(PageSeed.RoleKey)]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var role = await _context.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == id, ct);
        if (role is null) return NotFound();

        var userCount = await _context.UserRoles.CountAsync(ur => ur.RoleId == id, ct);
        var permissions = await _permissionService.GetPermissionsForRoleAsync(id, ct);

        var dto = new RoleDetailDto
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description,
            UserCount = userCount,
            IsSystem = role.Id == RoleSeed.SuperAdminRoleId,
            Permissions = permissions,
            Audit = ToAudit(role)
        };
        return View(dto);
    }

    [HttpGet]
    [PagePermission(PageSeed.RoleKey, "edit")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var role = await _context.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == id, ct);
        if (role is null) return NotFound();

        var permissions = await _permissionService.GetPermissionsForRoleAsync(id, ct);

        var dto = new RoleUpdateDto
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description,
            Permissions = permissions
        };

        ViewBag.IsSystem = role.Id == RoleSeed.SuperAdminRoleId;
        ViewBag.Audit = ToAudit(role);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.RoleKey, "edit")]
    public async Task<IActionResult> Edit(RoleUpdateDto dto, CancellationToken ct)
    {
        var role = await _context.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == dto.Id, ct);
        if (role is null) return NotFound();

        ViewBag.IsSystem = role.Id == RoleSeed.SuperAdminRoleId;
        ViewBag.Audit = ToAudit(role);

        await NormalizePermissionsAsync(dto.Permissions, ct);

        if (!ModelState.IsValid) return View(dto);

        if (role.Id != RoleSeed.SuperAdminRoleId)
        {
            role.Name = dto.Name;
            role.Description = dto.Description;
            var updateResult = await _roleManager.UpdateAsync(role);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(dto);
            }
        }

        await _permissionService.SaveRolePermissionsAsync(role.Id, dto.Permissions, ct);
        TempData["Success"] = "Rol başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.RoleKey, "delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (id == RoleSeed.SuperAdminRoleId)
        {
            TempData["Error"] = "Süper Admin rolü silinemez.";
            return RedirectToAction(nameof(Index));
        }

        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null) return NotFound();

        role.IsDeleted = true;
        await _roleManager.UpdateAsync(role);
        TempData["Success"] = "Rol silindi (soft delete).";
        return RedirectToAction(nameof(Index));
    }

    private async Task NormalizePermissionsAsync(List<RolePagePermissionDto> permissions, CancellationToken ct)
    {
        if (permissions is null || permissions.Count == 0)
        {
            permissions ??= new List<RolePagePermissionDto>();
            permissions.Clear();
            permissions.AddRange(await _permissionService.GetPermissionTemplateAsync(ct));
            return;
        }

        var template = await _permissionService.GetPermissionTemplateAsync(ct);
        foreach (var item in permissions)
        {
            if (item.PageId == Guid.Empty || string.IsNullOrWhiteSpace(item.PageName))
            {
                var match = template.FirstOrDefault(t =>
                    t.PageId == item.PageId ||
                    string.Equals(t.PageKey, item.PageKey, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    item.PageId = match.PageId;
                    item.PageKey = match.PageKey;
                    item.PageName = match.PageName;
                }
            }
        }
    }

    private static AuditInfoDto ToAudit(AppRole role) => new()
    {
        CreatedAt = role.CreatedAt,
        CreatedByName = role.CreatedByName,
        UpdatedAt = role.UpdatedAt,
        UpdatedByName = role.UpdatedByName,
        IsDeleted = role.IsDeleted,
        DeletedAt = role.DeletedAt,
        DeletedByName = role.DeletedByName
    };
}

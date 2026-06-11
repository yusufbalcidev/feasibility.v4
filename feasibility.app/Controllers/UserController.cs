using feasibility.Business.Middlewares;
using feasibility.DataAccess.Context;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Dtos.User;
using feasibility.Entity.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace feasibility.App.Controllers;

[PagePermission(PageSeed.UserKey)]
public class UserController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly AppDbContext _context;

    public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    private const int PageSize = 10;

    public async Task<IActionResult> Index(string? q, int page = 1, CancellationToken ct = default)
    {
        if (page < 1) page = 1;

        var query = _context.Users.IgnoreQueryFilters().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(u =>
                u.FirstName.Contains(term) ||
                u.LastName.Contains(term) ||
                (u.UserName != null && u.UserName.Contains(term)) ||
                (u.Email != null && u.Email.Contains(term)));
        }

        var totalCount = await query.CountAsync(ct);
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(ct);

        var userIds = users.Select(u => u.Id).ToList();
        var userRoles = await _context.UserRoles
            .Where(ur => userIds.Contains(ur.UserId))
            .ToListAsync(ct);
        var roleIds = userRoles.Select(ur => ur.RoleId).Distinct().ToList();
        var roles = await _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, ct);

        var list = users.Select(u =>
        {
            var roleId = userRoles.FirstOrDefault(ur => ur.UserId == u.Id)?.RoleId;
            return new UserListDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                RoleId = roleId,
                RoleName = roleId.HasValue && roles.TryGetValue(roleId.Value, out var r) ? r.Name : null,
                IsDeleted = u.IsDeleted,
                CreatedAt = u.CreatedAt,
                CreatedByName = u.CreatedByName
            };
        }).ToList();

        var model = PagedResult<UserListDto>.Create(list, totalCount, page, PageSize, q);
        return View(model);
    }

    [HttpGet]
    [PagePermission(PageSeed.UserKey, "create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateRoleSelectListAsync(ct);
        return View(new UserCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.UserKey, "create")]
    public async Task<IActionResult> Create(UserCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateRoleSelectListAsync(ct, dto.RoleId);
            return View(dto);
        }

        var user = new AppUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            await PopulateRoleSelectListAsync(ct, dto.RoleId);
            return View(dto);
        }

        var role = await _roleManager.FindByIdAsync(dto.RoleId.ToString());
        if (role is not null)
            await _userManager.AddToRoleAsync(user, role.Name!);

        TempData["Success"] = "Kullanıcı başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PagePermission(PageSeed.UserKey)]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return NotFound();

        var roleId = await _context.UserRoles
            .Where(ur => ur.UserId == id)
            .Select(ur => ur.RoleId)
            .FirstOrDefaultAsync(ct);

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId, ct);

        var dto = new UserDetailDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            RoleId = role?.Id,
            RoleName = role?.Name,
            Audit = new AuditInfoDto
            {
                CreatedAt = user.CreatedAt,
                CreatedByName = user.CreatedByName,
                UpdatedAt = user.UpdatedAt,
                UpdatedByName = user.UpdatedByName,
                IsDeleted = user.IsDeleted,
                DeletedAt = user.DeletedAt,
                DeletedByName = user.DeletedByName
            }
        };
        return View(dto);
    }

    [HttpGet]
    [PagePermission(PageSeed.UserKey, "edit")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return NotFound();

        var roleId = await _context.UserRoles
            .Where(ur => ur.UserId == id)
            .Select(ur => ur.RoleId)
            .FirstOrDefaultAsync(ct);

        var dto = new UserUpdateDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            RoleId = roleId
        };

        ViewBag.Audit = new AuditInfoDto
        {
            CreatedAt = user.CreatedAt,
            CreatedByName = user.CreatedByName,
            UpdatedAt = user.UpdatedAt,
            UpdatedByName = user.UpdatedByName,
            IsDeleted = user.IsDeleted,
            DeletedAt = user.DeletedAt,
            DeletedByName = user.DeletedByName
        };
        await PopulateRoleSelectListAsync(ct, dto.RoleId);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.UserKey, "edit")]
    public async Task<IActionResult> Edit(UserUpdateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateRoleSelectListAsync(ct, dto.RoleId);
            return View(dto);
        }

        var user = await _userManager.FindByIdAsync(dto.Id.ToString());
        if (user is null) return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.UserName = dto.UserName;
        user.Email = dto.Email;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            await PopulateRoleSelectListAsync(ct, dto.RoleId);
            return View(dto);
        }

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                await PopulateRoleSelectListAsync(ct, dto.RoleId);
                return View(dto);
            }
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        var role = await _roleManager.FindByIdAsync(dto.RoleId.ToString());
        if (role is not null)
            await _userManager.AddToRoleAsync(user, role.Name!);

        TempData["Success"] = "Kullanıcı başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.UserKey, "delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        if (user.Id == UserSeed.SuperAdminUserId)
        {
            TempData["Error"] = "Süper admin kullanıcısı silinemez.";
            return RedirectToAction(nameof(Index));
        }

        user.IsDeleted = true;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = "Kullanıcı silindi (soft delete).";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateRoleSelectListAsync(CancellationToken ct, Guid? selected = null)
    {
        var roles = await _context.Roles
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync(ct);
        ViewBag.Roles = roles.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = r.Name,
            Selected = selected.HasValue && selected.Value == r.Id
        }).ToList();
    }
}

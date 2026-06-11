using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Dtos.Location;
using feasibility.Entity.Entities.Locations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace feasibility.App.Controllers;

[PagePermission(PageSeed.LocationKey)]
public class LocationController : Controller
{
    private readonly IGenericService<Location> _locationService;
    private readonly IGenericService<LocationTypeMaintenance> _typeService;

    public LocationController(
        IGenericService<Location> locationService,
        IGenericService<LocationTypeMaintenance> typeService)
    {
        _locationService = locationService;
        _typeService = typeService;
    }

    private const int PageSize = 10;

    public async Task<IActionResult> Index(string? q, int page = 1, CancellationToken ct = default)
    {
        if (page < 1) page = 1;

        var query = _locationService.Query(ignoreFilters: true)
            .Include(l => l.LocationTypeMaintenance)
            .Select(l => new LocationListDto
            {
                Id = l.Id,
                Name = l.Name,
                City = l.City,
                District = l.District,
                LocationTypeMaintenanceId = l.LocationTypeMaintenanceId,
                LocationTypeMaintenanceName = l.LocationTypeMaintenance != null ? l.LocationTypeMaintenance.Name : null,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                IsDeleted = l.IsDeleted,
                CreatedAt = l.CreatedAt,
                CreatedByName = l.CreatedByName
            });

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(l =>
                l.Name.Contains(term) ||
                l.City.Contains(term) ||
                l.District.Contains(term) ||
                (l.LocationTypeMaintenanceName != null && l.LocationTypeMaintenanceName.Contains(term)) ||
                (l.CreatedByName != null && l.CreatedByName.Contains(term)));
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(ct);

        // Harita sekmesi tüm aktif lokasyonları gösterir (sayfalamadan bağımsız).
        var mapPoints = await _locationService.Query()
            .Where(l => l.Latitude != 0 && l.Longitude != 0)
            .Select(l => new LocationListDto
            {
                Id = l.Id,
                Name = l.Name,
                City = l.City,
                District = l.District,
                LocationTypeMaintenanceName = l.LocationTypeMaintenance != null ? l.LocationTypeMaintenance.Name : null,
                Latitude = l.Latitude,
                Longitude = l.Longitude
            })
            .ToListAsync(ct);
        ViewBag.MapPoints = mapPoints;

        var model = PagedResult<LocationListDto>.Create(items, totalCount, page, PageSize, q);
        return View(model);
    }

    [HttpGet]
    [PagePermission(PageSeed.LocationKey, "create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateTypeSelectListAsync(ct);
        return View(new LocationCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.LocationKey, "create")]
    public async Task<IActionResult> Create(LocationCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateTypeSelectListAsync(ct);
            return View(dto);
        }

        var entity = new Location
        {
            Name = dto.Name,
            Description = dto.Description,
            City = dto.City,
            District = dto.District,
            Address = dto.Address,
            Latitude = dto.Latitude!.Value,
            Longitude = dto.Longitude!.Value,
            LocationTypeMaintenanceId = dto.LocationTypeMaintenanceId
        };

        await _locationService.AddAsync(entity, ct);
        TempData["Success"] = "Lokasyon başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PagePermission(PageSeed.LocationKey)]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var entity = await _locationService.Query(ignoreFilters: true)
            .Include(l => l.LocationTypeMaintenance)
            .FirstOrDefaultAsync(l => l.Id == id, ct);
        if (entity is null) return NotFound();

        var dto = new LocationDetailDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description ?? string.Empty,
            City = entity.City,
            District = entity.District,
            Address = entity.Address,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            LocationTypeMaintenanceId = entity.LocationTypeMaintenanceId,
            LocationTypeMaintenanceName = entity.LocationTypeMaintenance?.Name,
            Audit = ToAudit(entity)
        };
        return View(dto);
    }

    [HttpGet]
    [PagePermission(PageSeed.LocationKey, "edit")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var entity = await _locationService.GetByIdAsync(id, ignoreFilters: true, ct: ct);
        if (entity is null) return NotFound();

        var dto = new LocationUpdateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            City = entity.City,
            District = entity.District,
            Address = entity.Address,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            LocationTypeMaintenanceId = entity.LocationTypeMaintenanceId
        };

        await PopulateTypeSelectListAsync(ct, dto.LocationTypeMaintenanceId);
        ViewBag.Audit = ToAudit(entity);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.LocationKey, "edit")]
    public async Task<IActionResult> Edit(LocationUpdateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateTypeSelectListAsync(ct, dto.LocationTypeMaintenanceId);
            return View(dto);
        }

        var entity = await _locationService.GetByIdAsync(dto.Id, ignoreFilters: true, ct: ct);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.City = dto.City;
        entity.District = dto.District;
        entity.Address = dto.Address;
        entity.Latitude = dto.Latitude!.Value;
        entity.Longitude = dto.Longitude!.Value;
        entity.LocationTypeMaintenanceId = dto.LocationTypeMaintenanceId;

        await _locationService.UpdateAsync(entity, ct);
        TempData["Success"] = "Lokasyon başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.LocationKey, "delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _locationService.SoftDeleteAsync(id, ct);
        TempData["Success"] = "Lokasyon silindi (soft delete).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.LocationKey, "edit")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        await _locationService.RestoreAsync(id, ct);
        TempData["Success"] = "Lokasyon aktif hale getirildi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateTypeSelectListAsync(CancellationToken ct, Guid? selected = null)
    {
        var types = await _typeService.GetAllAsync(ct: ct);
        ViewBag.LocationTypes = types
            .OrderBy(t => t.Name)
            .Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name,
                Selected = selected.HasValue && selected.Value == t.Id
            }).ToList();
    }

    private static AuditInfoDto ToAudit(Location e) => new()
    {
        CreatedAt = e.CreatedAt,
        CreatedByName = e.CreatedByName,
        UpdatedAt = e.UpdatedAt,
        UpdatedByName = e.UpdatedByName,
        IsDeleted = e.IsDeleted,
        DeletedAt = e.DeletedAt,
        DeletedByName = e.DeletedByName
    };
}

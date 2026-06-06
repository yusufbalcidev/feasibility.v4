using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Dtos.LocationTypeMaintenance;
using feasibility.Entity.Entities.Locations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace feasibility.App.Controllers;

[PagePermission(PageSeed.LocationTypeMaintenanceKey)]
public class LocationTypeMaintenanceController : Controller
{
    private readonly IGenericService<LocationTypeMaintenance> _service;

    public LocationTypeMaintenanceController(IGenericService<LocationTypeMaintenance> service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var list = await _service.Query(ignoreFilters: true)
            .Select(t => new LocationTypeMaintenanceListDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsDeleted = t.IsDeleted,
                CreatedAt = t.CreatedAt,
                CreatedByName = t.CreatedByName,
                LocationCount = t.Locations.Count(l => !l.IsDeleted)
            })
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
        return View(list);
    }

    [HttpGet]
    [PagePermission(PageSeed.LocationTypeMaintenanceKey, "create")]
    public IActionResult Create() => View(new LocationTypeMaintenanceCreateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.LocationTypeMaintenanceKey, "create")]
    public async Task<IActionResult> Create(LocationTypeMaintenanceCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);

        await _service.AddAsync(new LocationTypeMaintenance
        {
            Name = dto.Name,
            Description = dto.Description
        }, ct);
        TempData["Success"] = "Lokasyon tipi başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PagePermission(PageSeed.LocationTypeMaintenanceKey)]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var entity = await _service.Query(ignoreFilters: true)
            .Include(t => t.Locations)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
        if (entity is null) return NotFound();

        var dto = new LocationTypeMaintenanceDetailDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            LocationCount = entity.Locations.Count(l => !l.IsDeleted),
            Audit = new AuditInfoDto
            {
                CreatedAt = entity.CreatedAt,
                CreatedByName = entity.CreatedByName,
                UpdatedAt = entity.UpdatedAt,
                UpdatedByName = entity.UpdatedByName,
                IsDeleted = entity.IsDeleted,
                DeletedAt = entity.DeletedAt,
                DeletedByName = entity.DeletedByName
            }
        };
        return View(dto);
    }

    [HttpGet]
    [PagePermission(PageSeed.LocationTypeMaintenanceKey, "edit")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var entity = await _service.GetByIdAsync(id, ignoreFilters: true, ct: ct);
        if (entity is null) return NotFound();

        ViewBag.Audit = new AuditInfoDto
        {
            CreatedAt = entity.CreatedAt,
            CreatedByName = entity.CreatedByName,
            UpdatedAt = entity.UpdatedAt,
            UpdatedByName = entity.UpdatedByName,
            IsDeleted = entity.IsDeleted,
            DeletedAt = entity.DeletedAt,
            DeletedByName = entity.DeletedByName
        };
        return View(new LocationTypeMaintenanceUpdateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.LocationTypeMaintenanceKey, "edit")]
    public async Task<IActionResult> Edit(LocationTypeMaintenanceUpdateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);

        var entity = await _service.GetByIdAsync(dto.Id, ignoreFilters: true, ct: ct);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        entity.Description = dto.Description;

        await _service.UpdateAsync(entity, ct);
        TempData["Success"] = "Lokasyon tipi başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.LocationTypeMaintenanceKey, "delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.SoftDeleteAsync(id, ct);
        TempData["Success"] = "Lokasyon tipi silindi.";
        return RedirectToAction(nameof(Index));
    }
}

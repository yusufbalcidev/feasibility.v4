using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.Location;
using feasibility.Entity.Entities.Identity;
using feasibility.Entity.Entities.Locations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace feasibility.App.Controllers;

[PagePermission(PageSeed.DashboardKey)]
public class DashboardController : Controller
{
    private readonly IGenericService<Location> _locations;
    private readonly IGenericService<LocationTypeMaintenance> _types;
    private readonly IGenericService<AppUser> _users;

    public DashboardController(
        IGenericService<Location> locations,
        IGenericService<LocationTypeMaintenance> types,
        IGenericService<AppUser> users)
    {
        _locations = locations;
        _types = types;
        _users = users;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.LocationCount = await _locations.CountAsync(ct: ct);
        ViewBag.LocationTypeCount = await _types.CountAsync(ct: ct);
        ViewBag.UserCount = await _users.CountAsync(u => !u.IsDeleted, ct: ct);

        ViewBag.RecentLocations = await _locations.Query()
            .Include(l => l.LocationTypeMaintenance)
            .OrderByDescending(l => l.CreatedAt)
            .Take(5)
            .Select(l => new LocationListDto
            {
                Id = l.Id,
                Name = l.Name,
                City = l.City,
                District = l.District,
                LocationTypeMaintenanceId = l.LocationTypeMaintenanceId,
                LocationTypeMaintenanceName = l.LocationTypeMaintenance != null ? l.LocationTypeMaintenance.Name : null,
                IsDeleted = l.IsDeleted,
                CreatedAt = l.CreatedAt,
                CreatedByName = l.CreatedByName
            })
            .ToListAsync(ct);

        return View();
    }
}

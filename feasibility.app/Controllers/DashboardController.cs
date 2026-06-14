using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Entities.Identity;
using Microsoft.AspNetCore.Mvc;

namespace feasibility.App.Controllers;

[PagePermission(PageSeed.DashboardKey)]
public class DashboardController : Controller
{
    private readonly IFeasibilityAmortizationService _feasibilityService;
    private readonly IGenericService<AppUser> _users;

    public DashboardController(
        IFeasibilityAmortizationService feasibilityService,
        IGenericService<AppUser> users)
    {
        _feasibilityService = feasibilityService;
        _users = users;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.UserCount = await _users.CountAsync(u => !u.IsDeleted, ct: ct);

        var allFeasibilities = await _feasibilityService.GetListAsync(ct);

        var active = allFeasibilities.Where(f => !f.IsDeleted).ToList();

        ViewBag.FeasibilityCount = allFeasibilities.Count;
        ViewBag.ActiveCount = active.Count;
        ViewBag.PassiveCount = allFeasibilities.Count - active.Count;

        ViewBag.LocationCount = active
            .Where(f => !string.IsNullOrWhiteSpace(f.LocationName))
            .Select(f => f.LocationName.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        // Son 30 gün içinde eklenenler (ivme göstergesi)
        var since = DateTime.UtcNow.AddDays(-30);
        ViewBag.NewLast30Days = allFeasibilities.Count(f => f.CreatedAt >= since);

        ViewBag.RecentFeasibilities = allFeasibilities
            .OrderByDescending(f => f.CreatedAt)
            .Take(8)
            .ToList();

        return View();
    }
}

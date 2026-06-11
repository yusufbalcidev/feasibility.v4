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
        ViewBag.FeasibilityCount = allFeasibilities.Count;
        ViewBag.RecentFeasibilities = allFeasibilities
            .OrderByDescending(f => f.CreatedAt)
            .Take(10)
            .ToList();

        return View();
    }
}

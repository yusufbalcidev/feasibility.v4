using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using Microsoft.AspNetCore.Mvc;

namespace feasibility.App.Controllers;

[PagePermission(PageSeed.ActivityLogKey)]
public class ActivityLogController : Controller
{
    private readonly IActivityLogService _logService;

    public ActivityLogController(IActivityLogService logService)
    {
        _logService = logService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var logs = await _logService.GetListAsync(500, ct);
        return View(logs);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var dto = await _logService.GetDetailAsync(id, ct);
        if (dto is null) return NotFound();
        return View(dto);
    }
}

using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.ActivityLog;
using feasibility.Entity.Dtos.Common;
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

    private const int PageSize = 10;

    public async Task<IActionResult> Index(string? q, int page = 1, CancellationToken ct = default)
    {
        if (page < 1) page = 1;

        var logs = await _logService.GetListAsync(500, ct);

        IEnumerable<ActivityLogListDto> filtered = logs;
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            filtered = logs.Where(l =>
                (l.UserName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (l.UserEmail?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (l.Controller?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (l.Action?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (l.ActionType?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (l.IpAddress?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                l.HttpMethod.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var filteredList = filtered.ToList();
        var totalCount = filteredList.Count;
        var items = filteredList
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        var model = PagedResult<ActivityLogListDto>.Create(items, totalCount, page, PageSize, q);
        return View(model);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var dto = await _logService.GetDetailAsync(id, ct);
        if (dto is null) return NotFound();
        return View(dto);
    }
}

using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Dtos.FeasibilityAmortization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace feasibility.App.Controllers
{
    [PagePermission(PageSeed.FeasibilityKey)]
    public class FeasibilityAmortization : Controller
    {
        private readonly IFeasibilityAmortizationService _feasibilityAmortizationService;

        public FeasibilityAmortization(IFeasibilityAmortizationService feasibilityAmortizationService)
        {
            _feasibilityAmortizationService = feasibilityAmortizationService;
        }

        private const int PageSize = 10;

        private string FirstError()
            => ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m))
               ?? "Geçersiz veri.";

        public async Task<IActionResult> Index(string? q, int page = 1, CancellationToken ct = default)
        {
            if (page < 1) page = 1;

            var all = await _feasibilityAmortizationService.GetListAsync(ct);

            IEnumerable<FeasibilityAmortizationListDto> filtered = all;
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                filtered = all.Where(x =>
                    (x.FeasibilityName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.LocationName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.CreatedByName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    x.DeviceTypes.Any(d => d.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }

            var filteredList = filtered.ToList();
            var totalCount = filteredList.Count;
            var items = filteredList
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var model = PagedResult<FeasibilityAmortizationListDto>.Create(items, totalCount, page, PageSize, q);
            return View(model);
        }
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var formDataTask = _feasibilityAmortizationService.GetCreateFormDataAsync(ct);
            var infTask = _feasibilityAmortizationService.GetSonEnflasyonlarAsync(ct);

            await Task.WhenAll(formDataTask, infTask);

            var formData = formDataTask.Result;
            ViewBag.Locations = formData.Locations
                .Select(l => new SelectListItem { Value = l.Value, Text = l.Text })
                .ToList();
            ViewBag.FxJson = formData.FxJson;
            var (tl, usd, eur) = infTask.Result;
            ViewBag.TlInfJson = tl is not null ? tl.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "null";
            ViewBag.UsdInfJson = usd is not null ? usd.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "null";
            ViewBag.EurInfJson = eur is not null ? eur.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "null";
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetRates()
        {
            var json = await _feasibilityAmortizationService.GetRatesJsonAsync();
            return Json(System.Text.Json.JsonDocument.Parse(json).RootElement);
        }

        [HttpPost]
        [AutoValidation]
        public async Task<IActionResult> Calculate([FromBody] FeasibilityAmortizationSaveDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = FirstError() });

            try
            {
                var result = await _feasibilityAmortizationService.CalculatePreviewAsync(dto, ct);
                return PartialView("_ResultContent", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost("FeasibilityAmortization/CalcTest")]
        public async Task<IActionResult> CalcTest([FromBody] FeasibilityAmortizationSaveDto dto, CancellationToken ct)
        {
            var result = await _feasibilityAmortizationService.CalculatePreviewAsync(dto, ct);
            return Json(new
            {
                result.TotalInvestmentTl,
                result.TotalInvestmentUsd,
                result.AnnualNetProfitTl,
                result.AnnualNetProfitUsd,
                result.PaybackYears,
                result.RoiPercent,
                result.SunkInvestmentUsd,
                result.RecoverableInvestmentUsd,
                result.SunkPaybackYears,
                YearSummaries = result.YearSummaries,
            });
        }

        [HttpPost]
        [AutoValidation]
        public async Task<IActionResult> Save([FromBody] FeasibilityAmortizationSaveDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(new { message = FirstError(), errors });
            }

            try
            {
                var id = await _feasibilityAmortizationService.SaveStudyAsync(dto, ct);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            if (!await _feasibilityAmortizationService.IsLatestVersionAsync(id, ct))
                return RedirectToAction(nameof(View), new { id });

            var preloadJson = await _feasibilityAmortizationService.GetEditPreloadJsonAsync(id, ct);
            if (preloadJson is null) return NotFound();

            var formDataTask = _feasibilityAmortizationService.GetCreateFormDataAsync(ct);
            var infTask      = _feasibilityAmortizationService.GetSonEnflasyonlarAsync(ct);
            var detailTask   = _feasibilityAmortizationService.GetDetailAsync(id, ct);
            await Task.WhenAll(formDataTask, infTask, detailTask);                      

            var formData = formDataTask.Result;
            ViewBag.Locations = formData.Locations
                .Select(l => new SelectListItem { Value = l.Value, Text = l.Text })
                .ToList();
            ViewBag.FxJson = formData.FxJson;
            var (tl, usd, eur) = infTask.Result;
            ViewBag.TlInfJson  = tl  is not null ? tl .Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "null";
            ViewBag.UsdInfJson = usd is not null ? usd.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "null";
            ViewBag.EurInfJson = eur is not null ? eur.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "null";
            ViewBag.StudyId       = id;
            ViewBag.PreloadJson   = preloadJson;
            return View(detailTask.Result);
        }

        [HttpPost]
        [AutoValidation]
        public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] FeasibilityAmortizationSaveDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = FirstError() });

            if (!await _feasibilityAmortizationService.IsLatestVersionAsync(id, ct))
                return BadRequest(new { message = "Bu fizibilitenin daha güncel bir versiyonu var; eski versiyon düzenlenemez." });

            try
            {
                await _feasibilityAmortizationService.UpdateStudyAsync(id, dto, ct);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id, CancellationToken ct)
        {
            var detail = await _feasibilityAmortizationService.GetDetailAsync(id, ct);
            if (detail is null) return NotFound();

            var preloadJson = await _feasibilityAmortizationService.GetEditPreloadJsonAsync(id, ct);

            var formData = await _feasibilityAmortizationService.GetCreateFormDataAsync(ct);
            ViewBag.Locations = formData.Locations
                .Select(l => new SelectListItem { Value = l.Value, Text = l.Text })
                .ToList();
            ViewBag.PreloadJson = preloadJson ?? "null";

            return base.View("View", detail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _feasibilityAmortizationService.DeleteAsync(id, ct);
            TempData["Success"] = "Fizibilite silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
        {
            await _feasibilityAmortizationService.RestoreAsync(id, ct);
            TempData["Success"] = "Fizibilite aktif hale getirildi.";
            return RedirectToAction(nameof(Index));
        }

    }
}

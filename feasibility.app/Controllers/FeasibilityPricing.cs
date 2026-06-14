using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Dtos.FeasibilityPricing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;

namespace feasibility.App.Controllers
{
    [PagePermission(PageSeed.FeasibilityKey)]
    public class FeasibilityPricing : Controller
    {
        private readonly IFeasibilityPricingService _feasibilityPricingService;

        public FeasibilityPricing(IFeasibilityPricingService feasibilityPricingService)
        {
            _feasibilityPricingService = feasibilityPricingService;
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

            var all = await _feasibilityPricingService.GetListAsync(ct);

            IEnumerable<FeasibilityPricingListDto> filtered = all;
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                filtered = all.Where(x =>
                    (x.FeasibilityName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.CreatedByName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    x.DeviceTypes.Any(d => d.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }

            var filteredList = filtered.ToList();
            var totalCount = filteredList.Count;
            var items = filteredList
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var model = PagedResult<FeasibilityPricingListDto>.Create(items, totalCount, page, PageSize, q);
            return View(model);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var formDataTask = _feasibilityPricingService.GetCreateFormDataAsync(ct);
            var infTask = _feasibilityPricingService.GetSonEnflasyonlarAsync(ct);

            await Task.WhenAll(formDataTask, infTask);

            var formData = formDataTask.Result;
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
            var json = await _feasibilityPricingService.GetRatesJsonAsync();
            return Json(System.Text.Json.JsonDocument.Parse(json).RootElement);
        }

        [HttpPost]
        [AutoValidation]
        public async Task<IActionResult> Calculate([FromBody] FeasibilityPricingSaveDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = FirstError() });

            try
            {
                var result = await _feasibilityPricingService.CalculatePreviewAsync(dto, ct);
                return PartialView("_ResultContent", result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [AutoValidation]
        public async Task<IActionResult> Save([FromBody] FeasibilityPricingSaveDto dto, CancellationToken ct)
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
                var id = await _feasibilityPricingService.SaveStudyAsync(dto, ct);
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
            if (!await _feasibilityPricingService.IsLatestVersionAsync(id, ct))
                return RedirectToAction(nameof(View), new { id });

            var preloadJson = await _feasibilityPricingService.GetEditPreloadJsonAsync(id, ct);
            if (preloadJson is null) return NotFound();

            var formDataTask = _feasibilityPricingService.GetCreateFormDataAsync(ct);
            var infTask      = _feasibilityPricingService.GetSonEnflasyonlarAsync(ct);
            var detailTask   = _feasibilityPricingService.GetDetailAsync(id, ct);
            await Task.WhenAll(formDataTask, infTask, detailTask);

            var formData = formDataTask.Result;
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
        public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] FeasibilityPricingSaveDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = FirstError() });

            if (!await _feasibilityPricingService.IsLatestVersionAsync(id, ct))
                return BadRequest(new { message = "Bu fizibilitenin daha güncel bir versiyonu var; eski versiyon düzenlenemez." });

            try
            {
                await _feasibilityPricingService.UpdateStudyAsync(id, dto, ct);
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
            var detail = await _feasibilityPricingService.GetDetailAsync(id, ct);
            if (detail is null) return NotFound();

            var preloadJson = await _feasibilityPricingService.GetEditPreloadJsonAsync(id, ct);
            ViewBag.PreloadJson = preloadJson ?? "null";

            return base.View("View", detail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _feasibilityPricingService.DeleteAsync(id, ct);
            TempData["Success"] = "Fizibilite silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
        {
            await _feasibilityPricingService.RestoreAsync(id, ct);
            TempData["Success"] = "Fizibilite aktif hale getirildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}

using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.FeasibilityAmortization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var dto = await _feasibilityAmortizationService.GetDetailAsync(id, ct);
            if (dto is null) return NotFound();
            return View(dto);
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _feasibilityAmortizationService.GetListAsync(ct);
            return View(list);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var formDataTask = _feasibilityAmortizationService.GetCreateFormDataAsync(ct);
            var tufeTask     = _feasibilityAmortizationService.GetSonTufeAsync(ct);

            await Task.WhenAll(formDataTask, tufeTask);

            var formData = formDataTask.Result;
            ViewBag.Locations = formData.Locations
                .Select(l => new SelectListItem { Value = l.Value, Text = l.Text })
                .ToList();
            ViewBag.FxJson   = formData.FxJson;
            ViewBag.TufeJson = tufeTask.Result.HasValue
                ? tufeTask.Result.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)
                : "null";
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetRates()
        {
            var json = await _feasibilityAmortizationService.GetRatesJsonAsync();
            return Json(System.Text.Json.JsonDocument.Parse(json).RootElement);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] FeasibilityAmortizationSaveDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Geçersiz veri." });

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
            var preloadJson = await _feasibilityAmortizationService.GetEditPreloadJsonAsync(id, ct);
            if (preloadJson is null) return NotFound();

            var formDataTask = _feasibilityAmortizationService.GetCreateFormDataAsync(ct);
            var tufeTask     = _feasibilityAmortizationService.GetSonTufeAsync(ct);
            await Task.WhenAll(formDataTask, tufeTask);

            var formData = formDataTask.Result;
            ViewBag.Locations = formData.Locations
                .Select(l => new SelectListItem { Value = l.Value, Text = l.Text })
                .ToList();
            ViewBag.FxJson        = formData.FxJson;
            ViewBag.TufeJson      = tufeTask.Result.HasValue
                ? tufeTask.Result.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)
                : "null";
            ViewBag.StudyId       = id;
            ViewBag.PreloadJson   = preloadJson;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] FeasibilityAmortizationSaveDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Geçersiz veri." });

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

        public IActionResult Result()
        {
            return View();
        }
    }
}

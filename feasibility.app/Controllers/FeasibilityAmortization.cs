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

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list =await _feasibilityAmortizationService.GetListAsync(ct);
            return View(list);
        }
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var formDataTask = _feasibilityAmortizationService.GetCreateFormDataAsync(ct);
            var infTask      = _feasibilityAmortizationService.GetSonEnflasyonlarAsync(ct);

            await Task.WhenAll(formDataTask, infTask);

            var formData = formDataTask.Result;
            ViewBag.Locations = formData.Locations
                .Select(l => new SelectListItem { Value = l.Value, Text = l.Text })
                .ToList();
            ViewBag.FxJson = formData.FxJson;
            var (tl, usd, eur) = infTask.Result;
            ViewBag.TlInfJson  = tl  is not null ? tl .Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "null";
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
        public async Task<IActionResult> Calculate([FromBody] FeasibilityAmortizationSaveDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Geçersiz veri." });

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

        // GEÇİCİ TEST UCU — hesap doğrulaması için. Doğrulama sonrası kaldırılacak.
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
                return BadRequest(new { message = "Geçersiz veri.", errors });
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
            // Eski versiyon salt-okunur: doğrudan URL ile gelinse bile düzenleme açılmaz, görüntülemeye yönlendirilir.
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
            // Düzenleme sayfasındaki "Fizibilite Sonuçları" sekmesini ilk açılışta dolu getir.
            return View(detailTask.Result);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] FeasibilityAmortizationSaveDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Geçersiz veri." });

            // Eski versiyon salt-okunur: güncelleme isteği gelse bile reddedilir.
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

        // Salt-okunur görüntüleme: veri girişleri (düzenlenemez) + sonuçlar sekmeli.
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

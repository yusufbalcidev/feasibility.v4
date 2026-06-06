using feasibility.Business.Abstract;
using feasibility.Business.Middlewares;
using feasibility.DataAccess.Seeds;
using feasibility.Entity.Dtos.Common;
using feasibility.Entity.Dtos.Study;
using feasibility.Entity.Entities.FeasibilityAmortization;
using feasibility.Entity.Entities.Locations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace feasibility.App.Controllers;

[PagePermission(PageSeed.FeasibilityKey)]
public class StudyController : Controller
{
    private readonly IGenericService<Study> _studyService;
    private readonly IGenericService<Location> _locationService;

    public StudyController(
        IGenericService<Study> studyService,
        IGenericService<Location> locationService)
    {
        _studyService = studyService;
        _locationService = locationService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var studies = await _studyService.Query(ignoreFilters: true)
            .Include(s => s.Location)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new StudyListDto
            {
                Id = s.Id,
                LocationId = s.LocationId,
                LocationName = s.Location != null ? s.Location.Name : string.Empty,
                FeasibilityName = s.FeasibilityName,
                Kind = s.Kind,
                UsdRate = s.UsdRate,
                EurRate = s.EurRate,
                IsDeleted = s.IsDeleted,
                CreatedAt = s.CreatedAt,
                CreatedByName = s.CreatedByName
            })
            .ToListAsync(ct);
        return View(studies);
    }

    [HttpGet]
    [PagePermission(PageSeed.FeasibilityKey, "create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateLocationSelectListAsync(ct);
        return View(new StudyCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.FeasibilityKey, "create")]
    public async Task<IActionResult> Create(StudyCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLocationSelectListAsync(ct);
            return View(dto);
        }

        var entity = new Study
        {
            LocationId = dto.LocationId,
            FeasibilityName = dto.FeasibilityName,
            Kind = dto.Kind,
            UsdRate = dto.UsdRate,
            EurRate = dto.EurRate,
            InflationTl = dto.InflationTl,
            InflationUsd = dto.InflationUsd,
            InflationEur = dto.InflationEur,
            HasRent = dto.HasRent,
            MonthlyRent = dto.MonthlyRent,
            RentCurrency = dto.RentCurrency,
            MonthlyRentTl = ComputeTl(dto.MonthlyRent, dto.RentCurrency, dto.UsdRate, dto.EurRate),
            PostWarrantyMaintenanceCost = dto.PostWarrantyMaintenanceCost,
            PostWarrantyMaintenanceCurrency = dto.PostWarrantyMaintenanceCurrency,
            PostWarrantyMaintenanceCostTl = ComputeTl(dto.PostWarrantyMaintenanceCost, dto.PostWarrantyMaintenanceCurrency, dto.UsdRate, dto.EurRate),
            AdvertisingRevenue = dto.AdvertisingRevenue,
            AdvertisingRevenueCurrency = dto.AdvertisingRevenueCurrency,
            AdvertisingRevenueTl = ComputeTl(dto.AdvertisingRevenue, dto.AdvertisingRevenueCurrency, dto.UsdRate, dto.EurRate),
            StationUnitCost = dto.StationUnitCost,
            StationUnitCostCurrency = dto.StationUnitCostCurrency,
            StationUnitCostTl = ComputeTl(dto.StationUnitCost, dto.StationUnitCostCurrency, dto.UsdRate, dto.EurRate),
            ProviderEntryFee = dto.ProviderEntryFee,
            ProviderEntryFeeCurrency = dto.ProviderEntryFeeCurrency,
            ProviderEntryFeeTl = ComputeTl(dto.ProviderEntryFee, dto.ProviderEntryFeeCurrency, dto.UsdRate, dto.EurRate),
            InfrastructureCost = dto.InfrastructureCost,
            InfrastructureCostCurrency = dto.InfrastructureCostCurrency,
            InfrastructureCostTl = ComputeTl(dto.InfrastructureCost, dto.InfrastructureCostCurrency, dto.UsdRate, dto.EurRate),
            DeviceUnitCost = dto.DeviceUnitCost,
            DeviceUnitCostCurrency = dto.DeviceUnitCostCurrency,
            DeviceUnitCostTl = ComputeTl(dto.DeviceUnitCost, dto.DeviceUnitCostCurrency, dto.UsdRate, dto.EurRate),
            HasLoan = dto.HasLoan,
            LoanAmount = dto.LoanAmount,
            LoanAnnualInterestRate = dto.LoanAnnualInterestRate,
            LoanTermMonths = dto.LoanTermMonths
        };

        await _studyService.AddAsync(entity, ct);
        TempData["Success"] = "Fizibilite çalışması başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [PagePermission(PageSeed.FeasibilityKey)]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var entity = await _studyService.Query(ignoreFilters: true)
            .Include(s => s.Location)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is null) return NotFound();

        var dto = new StudyDetailDto
        {
            Id = entity.Id,
            LocationId = entity.LocationId,
            LocationName = entity.Location?.Name ?? string.Empty,
            FeasibilityName = entity.FeasibilityName,
            Kind = entity.Kind,
            UsdRate = entity.UsdRate,
            EurRate = entity.EurRate,
            InflationTl = entity.InflationTl,
            InflationUsd = entity.InflationUsd,
            InflationEur = entity.InflationEur,
            HasRent = entity.HasRent,
            MonthlyRent = entity.MonthlyRent,
            RentCurrency = entity.RentCurrency,
            MonthlyRentTl = entity.MonthlyRentTl,
            PostWarrantyMaintenanceCost = entity.PostWarrantyMaintenanceCost,
            PostWarrantyMaintenanceCurrency = entity.PostWarrantyMaintenanceCurrency,
            PostWarrantyMaintenanceCostTl = entity.PostWarrantyMaintenanceCostTl,
            AdvertisingRevenue = entity.AdvertisingRevenue,
            AdvertisingRevenueCurrency = entity.AdvertisingRevenueCurrency,
            AdvertisingRevenueTl = entity.AdvertisingRevenueTl,
            StationUnitCost = entity.StationUnitCost,
            StationUnitCostCurrency = entity.StationUnitCostCurrency,
            StationUnitCostTl = entity.StationUnitCostTl,
            ProviderEntryFee = entity.ProviderEntryFee,
            ProviderEntryFeeCurrency = entity.ProviderEntryFeeCurrency,
            ProviderEntryFeeTl = entity.ProviderEntryFeeTl,
            InfrastructureCost = entity.InfrastructureCost,
            InfrastructureCostCurrency = entity.InfrastructureCostCurrency,
            InfrastructureCostTl = entity.InfrastructureCostTl,
            DeviceUnitCost = entity.DeviceUnitCost,
            DeviceUnitCostCurrency = entity.DeviceUnitCostCurrency,
            DeviceUnitCostTl = entity.DeviceUnitCostTl,
            HasLoan = entity.HasLoan,
            LoanAmount = entity.LoanAmount,
            LoanAnnualInterestRate = entity.LoanAnnualInterestRate,
            LoanTermMonths = entity.LoanTermMonths,
            Audit = ToAudit(entity)
        };
        return View(dto);
    }

    [HttpGet]
    [PagePermission(PageSeed.FeasibilityKey, "edit")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var entity = await _studyService.GetByIdAsync(id, ignoreFilters: true, ct: ct);
        if (entity is null) return NotFound();

        var dto = new StudyUpdateDto
        {
            Id = entity.Id,
            LocationId = entity.LocationId,
            FeasibilityName = entity.FeasibilityName,
            Kind = entity.Kind,
            UsdRate = entity.UsdRate,
            EurRate = entity.EurRate,
            InflationTl = entity.InflationTl,
            InflationUsd = entity.InflationUsd,
            InflationEur = entity.InflationEur,
            HasRent = entity.HasRent,
            MonthlyRent = entity.MonthlyRent,
            RentCurrency = entity.RentCurrency,
            PostWarrantyMaintenanceCost = entity.PostWarrantyMaintenanceCost,
            PostWarrantyMaintenanceCurrency = entity.PostWarrantyMaintenanceCurrency,
            AdvertisingRevenue = entity.AdvertisingRevenue,
            AdvertisingRevenueCurrency = entity.AdvertisingRevenueCurrency,
            StationUnitCost = entity.StationUnitCost,
            StationUnitCostCurrency = entity.StationUnitCostCurrency,
            ProviderEntryFee = entity.ProviderEntryFee,
            ProviderEntryFeeCurrency = entity.ProviderEntryFeeCurrency,
            InfrastructureCost = entity.InfrastructureCost,
            InfrastructureCostCurrency = entity.InfrastructureCostCurrency,
            DeviceUnitCost = entity.DeviceUnitCost,
            DeviceUnitCostCurrency = entity.DeviceUnitCostCurrency,
            HasLoan = entity.HasLoan,
            LoanAmount = entity.LoanAmount,
            LoanAnnualInterestRate = entity.LoanAnnualInterestRate,
            LoanTermMonths = entity.LoanTermMonths
        };

        await PopulateLocationSelectListAsync(ct, dto.LocationId);
        ViewBag.Audit = ToAudit(entity);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.FeasibilityKey, "edit")]
    public async Task<IActionResult> Edit(StudyUpdateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLocationSelectListAsync(ct, dto.LocationId);
            return View(dto);
        }

        var entity = await _studyService.GetByIdAsync(dto.Id, ignoreFilters: true, ct: ct);
        if (entity is null) return NotFound();

        entity.LocationId = dto.LocationId;
        entity.FeasibilityName = dto.FeasibilityName;
        entity.Kind = dto.Kind;
        entity.UsdRate = dto.UsdRate;
        entity.EurRate = dto.EurRate;
        entity.InflationTl = dto.InflationTl;
        entity.InflationUsd = dto.InflationUsd;
        entity.InflationEur = dto.InflationEur;
        entity.HasRent = dto.HasRent;
        entity.MonthlyRent = dto.MonthlyRent;
        entity.RentCurrency = dto.RentCurrency;
        entity.MonthlyRentTl = ComputeTl(dto.MonthlyRent, dto.RentCurrency, dto.UsdRate, dto.EurRate);
        entity.PostWarrantyMaintenanceCost = dto.PostWarrantyMaintenanceCost;
        entity.PostWarrantyMaintenanceCurrency = dto.PostWarrantyMaintenanceCurrency;
        entity.PostWarrantyMaintenanceCostTl = ComputeTl(dto.PostWarrantyMaintenanceCost, dto.PostWarrantyMaintenanceCurrency, dto.UsdRate, dto.EurRate);
        entity.AdvertisingRevenue = dto.AdvertisingRevenue;
        entity.AdvertisingRevenueCurrency = dto.AdvertisingRevenueCurrency;
        entity.AdvertisingRevenueTl = ComputeTl(dto.AdvertisingRevenue, dto.AdvertisingRevenueCurrency, dto.UsdRate, dto.EurRate);
        entity.StationUnitCost = dto.StationUnitCost;
        entity.StationUnitCostCurrency = dto.StationUnitCostCurrency;
        entity.StationUnitCostTl = ComputeTl(dto.StationUnitCost, dto.StationUnitCostCurrency, dto.UsdRate, dto.EurRate);
        entity.ProviderEntryFee = dto.ProviderEntryFee;
        entity.ProviderEntryFeeCurrency = dto.ProviderEntryFeeCurrency;
        entity.ProviderEntryFeeTl = ComputeTl(dto.ProviderEntryFee, dto.ProviderEntryFeeCurrency, dto.UsdRate, dto.EurRate);
        entity.InfrastructureCost = dto.InfrastructureCost;
        entity.InfrastructureCostCurrency = dto.InfrastructureCostCurrency;
        entity.InfrastructureCostTl = ComputeTl(dto.InfrastructureCost, dto.InfrastructureCostCurrency, dto.UsdRate, dto.EurRate);
        entity.DeviceUnitCost = dto.DeviceUnitCost;
        entity.DeviceUnitCostCurrency = dto.DeviceUnitCostCurrency;
        entity.DeviceUnitCostTl = ComputeTl(dto.DeviceUnitCost, dto.DeviceUnitCostCurrency, dto.UsdRate, dto.EurRate);
        entity.HasLoan = dto.HasLoan;
        entity.LoanAmount = dto.LoanAmount;
        entity.LoanAnnualInterestRate = dto.LoanAnnualInterestRate;
        entity.LoanTermMonths = dto.LoanTermMonths;

        await _studyService.UpdateAsync(entity, ct);
        TempData["Success"] = "Fizibilite çalışması başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PagePermission(PageSeed.FeasibilityKey, "delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _studyService.SoftDeleteAsync(id, ct);
        TempData["Success"] = "Fizibilite çalışması silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLocationSelectListAsync(CancellationToken ct, Guid? selected = null)
    {
        var locations = await _locationService.GetAllAsync(ct: ct);
        ViewBag.Locations = locations
            .OrderBy(l => l.Name)
            .Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Name,
                Selected = selected.HasValue && selected.Value == l.Id
            }).ToList();
    }

    private static AuditInfoDto ToAudit(Study e) => new()
    {
        CreatedAt = e.CreatedAt,
        CreatedByName = e.CreatedByName,
        UpdatedAt = e.UpdatedAt,
        UpdatedByName = e.UpdatedByName,
        IsDeleted = e.IsDeleted,
        DeletedAt = e.DeletedAt,
        DeletedByName = e.DeletedByName
    };

    private static decimal ComputeTl(decimal amount, Entity.Entities.Enums.CurrencyType currency, decimal usdRate, decimal eurRate)
        => currency switch
        {
            Entity.Entities.Enums.CurrencyType.USD => amount * usdRate,
            Entity.Entities.Enums.CurrencyType.EUR => amount * eurRate,
            _ => amount
        };
}

using AutoMapper;
using feasibility.Business.Abstract;
using feasibility.Entity.Dtos.FeasibilityAmortization;
using feasibility.Entity.Dtos.Location;
using feasibility.Entity.Entities.Enums;
using feasibility.Entity.Entities.FeasibilityAmortization;
using feasibility.Entity.Entities.Locations;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace feasibility.Business.Concrete;

public class FeasibilityAmortizationManager : IFeasibilityAmortizationService
{
    private readonly IGenericService<Location>   _locationService;
    private readonly IGenericService<Study>      _studyService;
    private readonly IGenericService<DeviceLine> _deviceLineService;
    private readonly ITcmbService                _tcmb;
    private readonly IEvdsService                _evds;
    private readonly IMapper                     _mapper;

    public FeasibilityAmortizationManager(
        IGenericService<Location>   locationService,
        IGenericService<Study>      studyService,
        IGenericService<DeviceLine> deviceLineService,
        ITcmbService tcmb,
        IEvdsService evds,
        IMapper mapper)
    {
        _locationService   = locationService;
        _studyService      = studyService;
        _deviceLineService = deviceLineService;
        _tcmb              = tcmb;
        _evds              = evds;
        _mapper            = mapper;
    }

    public async Task<FeasibilityAmortizationCreateFormDto> GetCreateFormDataAsync(CancellationToken ct = default)
    {
        var locationsTask = _locationService.GetAllAsync(ct: ct);
        var ratesTask     = GetRatesJsonAsync(ct);

        await Task.WhenAll(locationsTask, ratesTask);

        var dtos = _mapper.Map<List<LocationListDto>>(locationsTask.Result);
        var selectItems = dtos.Select(l => new LocationSelectItem
        {
            Value = l.Id.ToString(),
            Text  = $"{l.Name} — {l.City} / {l.District}"
        }).ToList();

        return new FeasibilityAmortizationCreateFormDto
        {
            Locations = selectItems,
            FxJson    = ratesTask.Result,
        };
    }

    public async Task<string> GetRatesJsonAsync(CancellationToken ct = default)
    {
        try
        {
            var rates = await _tcmb.GetRatesAsync(ct);
            return JsonSerializer.Serialize(new
            {
                usdBuy  = rates.UsdBuy,
                usdSell = rates.UsdSell,
                eurBuy  = rates.EurBuy,
                eurSell = rates.EurSell,
            });
        }
        catch
        {
            return "{\"usdBuy\":0,\"usdSell\":0,\"eurBuy\":0,\"eurSell\":0}";
        }
    }

    public async Task<decimal?> GetSonTufeAsync(CancellationToken ct = default)
    {
        try
        {
            var baslangic = DateTime.Now.AddMonths(-2).ToString("01-MM-yyyy");
            var veri = await _evds.GetInflationAsync(
                seriKodu:  "TP.FG.J0",
                baslangic: baslangic,
                frekans:   5,
                formul:    "4",
                ct:        ct);

            return veri.Where(d => d.Deger.HasValue)
                       .OrderByDescending(d => d.Tarih)
                       .FirstOrDefault()?.Deger;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Guid> SaveStudyAsync(FeasibilityAmortizationSaveDto dto, CancellationToken ct = default)
    {
        var study = new Study
        {
            LocationId                      = dto.LocationId,
            FeasibilityName                 = dto.FeasibilityName,
            Kind                            = dto.Kind,
            UsdRate                         = dto.UsdRate,
            EurRate                         = dto.EurRate,
            InflationTl                     = dto.InflationTl,
            InflationUsd                    = dto.InflationUsd,
            InflationEur                    = dto.InflationEur,
            HasRent                         = dto.HasRent,
            MonthlyRent                     = dto.MonthlyRent,
            RentCurrency                    = dto.RentCurrency,
            MonthlyRentTl                   = ToTl(dto.MonthlyRent,                dto.RentCurrency,                    dto.UsdRate, dto.EurRate),
            PostWarrantyMaintenanceCost     = dto.PostWarrantyMaintenanceCost,
            PostWarrantyMaintenanceCurrency = dto.PostWarrantyMaintenanceCurrency,
            PostWarrantyMaintenanceCostTl   = ToTl(dto.PostWarrantyMaintenanceCost, dto.PostWarrantyMaintenanceCurrency, dto.UsdRate, dto.EurRate),
            AdvertisingRevenue              = dto.AdvertisingRevenue,
            AdvertisingRevenueCurrency      = dto.AdvertisingRevenueCurrency,
            AdvertisingRevenueTl            = ToTl(dto.AdvertisingRevenue,          dto.AdvertisingRevenueCurrency,      dto.UsdRate, dto.EurRate),
            StationUnitCost                 = dto.StationUnitCost,
            StationUnitCostCurrency         = dto.StationUnitCostCurrency,
            StationUnitCostTl               = ToTl(dto.StationUnitCost,             dto.StationUnitCostCurrency,         dto.UsdRate, dto.EurRate),
            ProviderEntryFee                = dto.ProviderEntryFee,
            ProviderEntryFeeCurrency        = dto.ProviderEntryFeeCurrency,
            ProviderEntryFeeTl              = ToTl(dto.ProviderEntryFee,            dto.ProviderEntryFeeCurrency,        dto.UsdRate, dto.EurRate),
            InfrastructureCost              = dto.InfrastructureCost,
            InfrastructureCostCurrency      = dto.InfrastructureCostCurrency,
            InfrastructureCostTl            = ToTl(dto.InfrastructureCost,          dto.InfrastructureCostCurrency,      dto.UsdRate, dto.EurRate),
            DeviceUnitCost                  = dto.DeviceUnitCost,
            DeviceUnitCostCurrency          = dto.DeviceUnitCostCurrency,
            DeviceUnitCostTl                = ToTl(dto.DeviceUnitCost,              dto.DeviceUnitCostCurrency,          dto.UsdRate, dto.EurRate),
            HasLoan                         = dto.HasLoan,
            LoanAmount                      = dto.LoanAmount,
            LoanAnnualInterestRate          = dto.LoanAnnualInterestRate,
            LoanTermMonths                  = dto.LoanTermMonths,
        };

        foreach (var lineDto in dto.DeviceLines)
        {
            var line = new DeviceLine
            {
                DeviceType               = lineDto.DeviceType,
                DeviceCount              = lineDto.DeviceCount,
                SocketCount              = lineDto.SocketCount,
                DailyChargesPerSocket    = lineDto.DailyChargesPerSocket,
                AvgKwh                   = lineDto.AvgKwh,
                SalePriceTl              = lineDto.SalePriceTl,
                PurchasePriceTl          = lineDto.PurchasePriceTl,
                UnitLocationCost         = lineDto.UnitLocationCost,
                UnitLocationCostCurrency = lineDto.UnitLocationCostCurrency,
                UnitLocationCostTl       = ToTl(lineDto.UnitLocationCost, lineDto.UnitLocationCostCurrency, dto.UsdRate, dto.EurRate),
                AgreementGenre           = lineDto.AgreementGenre,
                AgreementRate            = lineDto.AgreementRate,
            };

            foreach (var projDto in lineDto.YearProjections)
            {
                line.YearProjections.Add(new YearProjection
                {
                    Year                 = projDto.Year,
                    DailyChargePerSocket = projDto.DailyChargePerSocket,
                });
            }

            study.DeviceLines.Add(line);
        }

        // Study + DeviceLines + YearProjections tek bir nesne ağacı olarak,
        // tek SaveChanges ile insert edilir. EF, FK'leri (StudyId/DeviceLineId)
        // otomatik doldurur; ikinci bir Update gerekmez.
        await _studyService.AddAsync(study, ct);

        return study.Id;
    }

    public async Task<List<FeasibilityAmortizationListDto>> GetListAsync(CancellationToken ct = default)
    {
        // ignoreFilters: silinen (soft-delete) kayıtlar da listede "Pasif" olarak görünsün
        var studies = await _studyService.Query(ignoreFilters: true)
            .Where(s => s.Kind == FeasibilityKind.Amortization)
            .Include(s => s.Location)
            .Include(s => s.DeviceLines)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

        return studies.Select(s =>
        {
            // Toplam yatırım (TL): study geneli tek seferlik kalemler + cihaz/lokasyon bedelleri
            var oneTimeTl = s.StationUnitCostTl
                          + s.ProviderEntryFeeTl
                          + s.InfrastructureCostTl;

            var deviceTl = s.DeviceLines.Sum(d =>
                  d.UnitLocationCostTl
                + s.DeviceUnitCostTl * d.DeviceCount);

            var totalTl  = oneTimeTl + deviceTl;
            var totalUsd = s.UsdRate > 0 ? totalTl / s.UsdRate : 0m;

            return new FeasibilityAmortizationListDto
            {
                Id                 = s.Id,
                FeasibilityName    = s.FeasibilityName,
                LocationName       = s.Location != null
                    ? $"{s.Location.Name} — {s.Location.City} / {s.Location.District}"
                    : string.Empty,
                DeviceTypes        = s.DeviceLines
                    .OrderBy(d => d.DeviceType)
                    .Select(d => d.DeviceType.ToString())
                    .Distinct()
                    .ToList(),
                TotalInvestmentUsd = Math.Round(totalUsd, 0),
                IsDeleted          = s.IsDeleted,
                CreatedAt          = s.CreatedAt,
                CreatedByName      = s.CreatedByName,
            };
        }).ToList();
    }

    public async Task<FeasibilityAmortizationDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _studyService.Query(ignoreFilters: true)
            .Where(x => x.Id == id)
            .Include(x => x.Location)
            .Include(x => x.DeviceLines)
                .ThenInclude(d => d.YearProjections)
            .FirstOrDefaultAsync(ct);

        if (s == null) return null;

        var oneTimeTl = s.StationUnitCostTl + s.ProviderEntryFeeTl + s.InfrastructureCostTl;
        var deviceTl  = s.DeviceLines.Sum(d => d.UnitLocationCostTl + s.DeviceUnitCostTl * d.DeviceCount);
        var totalTl   = oneTimeTl + deviceTl;
        var totalUsd  = s.UsdRate > 0 ? totalTl / s.UsdRate : 0m;

        var annualRent        = s.HasRent ? s.MonthlyRentTl * 12 : 0m;
        var annualMaintenance = s.PostWarrantyMaintenanceCostTl;
        var annualAdvertising = s.AdvertisingRevenueTl;

        decimal annualLoanPayment = 0;
        if (s.HasLoan && s.LoanAmount > 0 && s.LoanTermMonths > 0)
        {
            var r = s.LoanAnnualInterestRate / 100m / 12m;
            if (r == 0)
            {
                annualLoanPayment = s.LoanAmount / s.LoanTermMonths * 12m;
            }
            else
            {
                var f = (decimal)Math.Pow((double)(1 + r), s.LoanTermMonths);
                var monthly = s.LoanAmount * r * f / (f - 1);
                annualLoanPayment = monthly * Math.Min(12, s.LoanTermMonths);
            }
        }

        var linesDtos = new List<DeviceLineDetailDto>();
        decimal totalRevTl  = 0;
        decimal totalElecTl = 0;
        decimal totalCommTl = 0;

        foreach (var d in s.DeviceLines.OrderBy(x => x.DeviceType))
        {
            var lineInvestment = d.UnitLocationCostTl + s.DeviceUnitCostTl * d.DeviceCount;

            var year1Daily    = d.YearProjections.OrderBy(y => y.Year).FirstOrDefault()?.DailyChargePerSocket ?? d.DailyChargesPerSocket;
            var annualCharges = d.SocketCount * year1Daily * 365m;
            var annualEnergy  = annualCharges * d.AvgKwh;
            var annualRev     = annualEnergy * d.SalePriceTl;
            var annualElec    = annualEnergy * d.PurchasePriceTl;
            var grossMargin   = annualRev - annualElec;

            var commission = d.AgreementGenre == Entity.Entities.Enums.AgreementGenre.Revenue
                ? d.AgreementRate * annualRev
                : d.AgreementRate * grossMargin;

            totalRevTl  += annualRev;
            totalElecTl += annualElec;
            totalCommTl += commission;

            var yearProjs = d.YearProjections.OrderBy(y => y.Year).Select(y =>
            {
                var yCharges = d.SocketCount * y.DailyChargePerSocket * 365m;
                var yEnergy  = yCharges * d.AvgKwh;
                return new YearProjectionDetailDto
                {
                    Year                    = y.Year,
                    DailyChargePerSocket    = y.DailyChargePerSocket,
                    AnnualRevenueTl         = yEnergy * d.SalePriceTl,
                    AnnualElectricityCostTl = yEnergy * d.PurchasePriceTl,
                };
            }).ToList();

            linesDtos.Add(new DeviceLineDetailDto
            {
                DeviceType              = d.DeviceType.ToString(),
                DeviceCount             = d.DeviceCount,
                SocketCount             = d.SocketCount,
                DailyChargesPerSocket   = d.DailyChargesPerSocket,
                AvgKwh                  = d.AvgKwh,
                SalePriceTl             = d.SalePriceTl,
                PurchasePriceTl         = d.PurchasePriceTl,
                UnitLocationCostTl      = d.UnitLocationCostTl,
                AgreementGenre          = d.AgreementGenre == Entity.Entities.Enums.AgreementGenre.Profit ? "Kâr" : "Ciro",
                AgreementRate           = d.AgreementRate,
                LineInvestmentTl        = lineInvestment,
                AnnualRevenueTl         = annualRev,
                AnnualElectricityCostTl = annualElec,
                AnnualCommissionTl      = commission,
                AnnualNetMarginTl       = grossMargin - commission,
                YearProjections         = yearProjs,
            });
        }

        var annualNetTl  = totalRevTl - totalElecTl - totalCommTl - annualRent - annualMaintenance + annualAdvertising - annualLoanPayment;
        var annualNetUsd = s.UsdRate > 0 ? annualNetTl / s.UsdRate : 0m;
        var payback      = annualNetUsd > 0 ? Math.Round(totalUsd / annualNetUsd, 1) : 0m;
        var roi          = totalUsd > 0 ? Math.Round(annualNetUsd / totalUsd * 100, 1) : 0m;

        return new FeasibilityAmortizationDetailDto
        {
            Id                       = s.Id,
            FeasibilityName          = s.FeasibilityName,
            LocationName             = s.Location != null ? $"{s.Location.Name} — {s.Location.City} / {s.Location.District}" : string.Empty,
            CreatedAt                = s.CreatedAt,
            CreatedByName            = s.CreatedByName,
            IsDeleted                = s.IsDeleted,
            UsdRate                  = s.UsdRate,
            EurRate                  = s.EurRate,
            InflationTl              = s.InflationTl,
            InflationUsd             = s.InflationUsd,
            InflationEur             = s.InflationEur,
            StationUnitCostTl        = s.StationUnitCostTl,
            ProviderEntryFeeTl       = s.ProviderEntryFeeTl,
            InfrastructureCostTl     = s.InfrastructureCostTl,
            DeviceTotalTl            = deviceTl,
            TotalInvestmentTl        = totalTl,
            TotalInvestmentUsd       = Math.Round(totalUsd, 0),
            HasRent                  = s.HasRent,
            AnnualRentTl             = annualRent,
            AnnualMaintenanceTl      = annualMaintenance,
            AnnualAdvertisingRevenueTl = annualAdvertising,
            HasLoan                  = s.HasLoan,
            LoanAmount               = s.LoanAmount,
            LoanAnnualInterestRate   = s.LoanAnnualInterestRate,
            LoanTermMonths           = s.LoanTermMonths,
            AnnualLoanPaymentTl      = annualLoanPayment,
            DeviceLines              = linesDtos,
            AnnualNetProfitTl        = annualNetTl,
            AnnualNetProfitUsd       = Math.Round(annualNetUsd, 0),
            PaybackYears             = payback,
            RoiPercent               = roi,
        };
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _studyService.SoftDeleteAsync(id, ct);

    public Task RestoreAsync(Guid id, CancellationToken ct = default)
        => _studyService.RestoreAsync(id, ct);

    public async Task<string?> GetEditPreloadJsonAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _studyService.Query(ignoreFilters: true)
            .Where(x => x.Id == id)
            .Include(x => x.DeviceLines)
                .ThenInclude(d => d.YearProjections)
            .FirstOrDefaultAsync(ct);

        if (s == null) return null;

        var ccyLabel = new[] { "TRY", "USD", "EUR" };

        var stationsJs = s.DeviceLines
            .Where(d => !d.IsDeleted)
            .OrderBy(d => d.DeviceType)
            .Select(d => new
            {
                type     = d.DeviceType == Entity.Entities.Enums.DeviceType.AC ? "AC" : "DC",
                adet     = d.DeviceCount,
                soket    = d.SocketCount,
                daily    = d.DailyChargesPerSocket,
                avgkwh   = d.AvgKwh,
                pTL      = d.SalePriceTl,
                alisKwh  = d.PurchasePriceTl,
                ccy      = ccyLabel[(int)d.UnitLocationCostCurrency],
                bedel    = d.UnitLocationCost,
                contract = d.AgreementGenre == Entity.Entities.Enums.AgreementGenre.Profit ? "Kâr" : "Ciro",
                oran     = d.AgreementRate * 100,
                proj     = d.YearProjections
                            .Where(y => !y.IsDeleted)
                            .OrderBy(y => y.Year)
                            .Select(y => new { year = y.Year, daily = y.DailyChargePerSocket })
                            .ToList(),
                touched  = true,
            }).ToList();

        var preload = new
        {
            locationId               = s.LocationId.ToString(),
            feasibilityName          = s.FeasibilityName,
            usdRate                  = s.UsdRate,
            eurRate                  = s.EurRate,
            inflTl                   = s.InflationTl,
            inflUsd                  = s.InflationUsd,
            inflEur                  = s.InflationEur,
            hasRent                  = s.HasRent,
            monthlyRent              = s.MonthlyRent,
            rentCurrency             = (int)s.RentCurrency,
            postWarrantyCost         = s.PostWarrantyMaintenanceCost,
            postWarrantyCurrency     = (int)s.PostWarrantyMaintenanceCurrency,
            advertisingRevenue       = s.AdvertisingRevenue,
            advertisingRevenueCurrency = (int)s.AdvertisingRevenueCurrency,
            stationCost              = s.StationUnitCost,
            stationCostCurrency      = (int)s.StationUnitCostCurrency,
            providerFee              = s.ProviderEntryFee,
            providerFeeCurrency      = (int)s.ProviderEntryFeeCurrency,
            infraCost                = s.InfrastructureCost,
            infraCostCurrency        = (int)s.InfrastructureCostCurrency,
            deviceCost               = s.DeviceUnitCost,
            deviceCostCurrency       = (int)s.DeviceUnitCostCurrency,
            hasLoan                  = s.HasLoan,
            loanAmount               = s.LoanAmount,
            loanRate                 = s.LoanAnnualInterestRate,
            loanMonths               = s.LoanTermMonths,
            stations                 = stationsJs,
        };

        return JsonSerializer.Serialize(preload, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    public async Task UpdateStudyAsync(Guid id, FeasibilityAmortizationSaveDto dto, CancellationToken ct = default)
    {
        var study = await _studyService.Query(ignoreFilters: true)
            .Where(s => s.Id == id)
            .Include(s => s.DeviceLines)
                .ThenInclude(d => d.YearProjections)
            .FirstOrDefaultAsync(ct);

        if (study == null) return;

        foreach (var line in study.DeviceLines)
        {
            foreach (var proj in line.YearProjections)
                proj.IsDeleted = true;
            line.IsDeleted = true;
        }

        study.LocationId                      = dto.LocationId;
        study.FeasibilityName                 = dto.FeasibilityName;
        study.UsdRate                         = dto.UsdRate;
        study.EurRate                         = dto.EurRate;
        study.InflationTl                     = dto.InflationTl;
        study.InflationUsd                    = dto.InflationUsd;
        study.InflationEur                    = dto.InflationEur;
        study.HasRent                         = dto.HasRent;
        study.MonthlyRent                     = dto.MonthlyRent;
        study.RentCurrency                    = dto.RentCurrency;
        study.MonthlyRentTl                   = ToTl(dto.MonthlyRent,                dto.RentCurrency,                    dto.UsdRate, dto.EurRate);
        study.PostWarrantyMaintenanceCost     = dto.PostWarrantyMaintenanceCost;
        study.PostWarrantyMaintenanceCurrency = dto.PostWarrantyMaintenanceCurrency;
        study.PostWarrantyMaintenanceCostTl   = ToTl(dto.PostWarrantyMaintenanceCost, dto.PostWarrantyMaintenanceCurrency, dto.UsdRate, dto.EurRate);
        study.AdvertisingRevenue              = dto.AdvertisingRevenue;
        study.AdvertisingRevenueCurrency      = dto.AdvertisingRevenueCurrency;
        study.AdvertisingRevenueTl            = ToTl(dto.AdvertisingRevenue,          dto.AdvertisingRevenueCurrency,      dto.UsdRate, dto.EurRate);
        study.StationUnitCost                 = dto.StationUnitCost;
        study.StationUnitCostCurrency         = dto.StationUnitCostCurrency;
        study.StationUnitCostTl               = ToTl(dto.StationUnitCost,             dto.StationUnitCostCurrency,         dto.UsdRate, dto.EurRate);
        study.ProviderEntryFee                = dto.ProviderEntryFee;
        study.ProviderEntryFeeCurrency        = dto.ProviderEntryFeeCurrency;
        study.ProviderEntryFeeTl              = ToTl(dto.ProviderEntryFee,            dto.ProviderEntryFeeCurrency,        dto.UsdRate, dto.EurRate);
        study.InfrastructureCost              = dto.InfrastructureCost;
        study.InfrastructureCostCurrency      = dto.InfrastructureCostCurrency;
        study.InfrastructureCostTl            = ToTl(dto.InfrastructureCost,          dto.InfrastructureCostCurrency,      dto.UsdRate, dto.EurRate);
        study.DeviceUnitCost                  = dto.DeviceUnitCost;
        study.DeviceUnitCostCurrency          = dto.DeviceUnitCostCurrency;
        study.DeviceUnitCostTl                = ToTl(dto.DeviceUnitCost,              dto.DeviceUnitCostCurrency,          dto.UsdRate, dto.EurRate);
        study.HasLoan                         = dto.HasLoan;
        study.LoanAmount                      = dto.LoanAmount;
        study.LoanAnnualInterestRate          = dto.LoanAnnualInterestRate;
        study.LoanTermMonths                  = dto.LoanTermMonths;

        foreach (var lineDto in dto.DeviceLines)
        {
            var line = new DeviceLine
            {
                DeviceType               = lineDto.DeviceType,
                DeviceCount              = lineDto.DeviceCount,
                SocketCount              = lineDto.SocketCount,
                DailyChargesPerSocket    = lineDto.DailyChargesPerSocket,
                AvgKwh                   = lineDto.AvgKwh,
                SalePriceTl              = lineDto.SalePriceTl,
                PurchasePriceTl          = lineDto.PurchasePriceTl,
                UnitLocationCost         = lineDto.UnitLocationCost,
                UnitLocationCostCurrency = lineDto.UnitLocationCostCurrency,
                UnitLocationCostTl       = ToTl(lineDto.UnitLocationCost, lineDto.UnitLocationCostCurrency, dto.UsdRate, dto.EurRate),
                AgreementGenre           = lineDto.AgreementGenre,
                AgreementRate            = lineDto.AgreementRate,
            };
            foreach (var projDto in lineDto.YearProjections)
                line.YearProjections.Add(new YearProjection { Year = projDto.Year, DailyChargePerSocket = projDto.DailyChargePerSocket });

            study.DeviceLines.Add(line);
        }

        await _studyService.SaveChangesAsync(ct);
    }

    private static decimal ToTl(decimal amount, Entity.Entities.Enums.CurrencyType currency, decimal usdRate, decimal eurRate)
        => currency switch
        {
            Entity.Entities.Enums.CurrencyType.USD => amount * usdRate,
            Entity.Entities.Enums.CurrencyType.EUR => amount * eurRate,
            _                                       => amount
        };
}

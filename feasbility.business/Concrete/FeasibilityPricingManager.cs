using AutoMapper;
using feasibility.Business.Abstract;
using feasibility.Entity.Dtos.FeasibilityPricing;
using feasibility.Entity.Entities.Enums;
using feasibility.Entity.Entities.FeasibilityPricing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace feasibility.Business.Concrete;

public class FeasibilityPricingManager : IFeasibilityPricingService
{
    private const decimal DaysPerYear = 365m;

    private readonly IGenericService<PricingStudy>   _studyService;
    private readonly ITcmbService                    _tcmb;
    private readonly IWorldBankService               _worldBank;
    private readonly IEvdsInflationService           _evds;
    private readonly IMapper                          _mapper;

    public FeasibilityPricingManager(
        IGenericService<PricingStudy> studyService,
        ITcmbService tcmb,
        IWorldBankService worldBank,
        IEvdsInflationService evds,
        IMapper mapper)
    {
        _studyService    = studyService;
        _tcmb      = tcmb;
        _worldBank = worldBank;
        _evds      = evds;
        _mapper    = mapper;
    }

    public async Task<FeasibilityPricingCreateFormDto> GetCreateFormDataAsync(CancellationToken ct = default)
    {
        var fxJson = await GetRatesJsonAsync(ct);

        return new FeasibilityPricingCreateFormDto
        {
            FxJson = fxJson,
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

    public async Task<(decimal? tl, decimal? usd, decimal? eur)> GetSonEnflasyonlarAsync(CancellationToken ct = default)
    {
        var tlEvdsTask = _evds.GetLatestTufeAnnualAsync(ct);
        var tlWbTask   = _worldBank.GetLatestInflationAsync("TR",  ct);
        var usdTask    = _worldBank.GetLatestInflationAsync("US",  ct);
        var eurTask    = _worldBank.GetLatestInflationAsync("EMU", ct);
        await Task.WhenAll(tlEvdsTask, tlWbTask, usdTask, eurTask);

        var tl = tlEvdsTask.Result ?? tlWbTask.Result;
        return (tl, usdTask.Result, eurTask.Result);
    }

    public async Task<Guid> SaveStudyAsync(FeasibilityPricingSaveDto dto, CancellationToken ct = default)
    {
        var study = _mapper.Map<PricingStudy>(dto);
        study.Version = dto.SaveAsNewVersion ? dto.BaseVersion + 1 : 1;

        foreach (var stationDto in dto.Stations)
            study.Stations.Add(MapStation(stationDto, dto.UsdRate, dto.EurRate));

        await _studyService.AddAsync(study, ct);
        return study.Id;
    }

    public async Task<List<FeasibilityPricingListDto>> GetListAsync(CancellationToken ct = default)
    {
        var studies = await _studyService.Query(ignoreFilters: true)
            .Where(s => s.Kind == FeasibilityKind.Pricing)
            .Include(s => s.Stations)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

        var latestByName = studies
            .Where(s => !s.IsDeleted)
            .GroupBy(s => s.FeasibilityName)
            .ToDictionary(g => g.Key, g => g.Max(x => x.Version));

        return studies.Select(s => new FeasibilityPricingListDto
        {
            Id              = s.Id,
            FeasibilityName = s.FeasibilityName,
            Version         = s.Version,
            DeviceTypes     = s.Stations
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.DeviceType)
                .Select(d => d.DeviceType.ToString())
                .Distinct()
                .ToList(),
            IsDeleted       = s.IsDeleted,
            IsLatest        = !latestByName.TryGetValue(s.FeasibilityName, out var maxV) || s.Version >= maxV,
            CreatedAt       = s.CreatedAt,
            CreatedByName   = s.CreatedByName,
        }).ToList();
    }

    public async Task<FeasibilityPricingDetailDto?> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _studyService.Query(ignoreFilters: true)
            .Where(x => x.Id == id)
            .Include(x => x.Stations)
            .FirstOrDefaultAsync(ct);

        return s == null ? null : BuildDetail(s);
    }

    public Task<FeasibilityPricingDetailDto> CalculatePreviewAsync(FeasibilityPricingSaveDto dto, CancellationToken ct = default)
    {
        var study = _mapper.Map<PricingStudy>(dto);

        foreach (var stationDto in dto.Stations)
            study.Stations.Add(MapStation(stationDto, dto.UsdRate, dto.EurRate));

        study.CreatedAt = DateTime.UtcNow;
        return Task.FromResult(BuildDetail(study));
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _studyService.SoftDeleteAsync(id, ct);

    public Task RestoreAsync(Guid id, CancellationToken ct = default)
        => _studyService.RestoreAsync(id, ct);

    public async Task<string?> GetEditPreloadJsonAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _studyService.Query(ignoreFilters: true)
            .Where(x => x.Id == id)
            .Include(x => x.Stations)
            .FirstOrDefaultAsync(ct);

        if (s == null) return null;

        var stationsJs = s.Stations
            .Where(d => !d.IsDeleted)
            .OrderBy(d => d.DeviceType)
            .Select(d => new
            {
                type          = d.DeviceType == DeviceType.AC ? "AC" : "DC",
                soket         = d.SocketCount,
                dailyKwh      = d.DailyKwhPerSocket,
                economicLife  = d.EconomicLifeYears,
                discountRate  = d.DiscountRate,
                targetProfit  = d.TargetProfitMargin,
                hardware      = d.HardwareCost,
                hardwareCcy   = (int)d.HardwareCostCurrency,
                infra         = d.InfrastructureCost,
                infraCcy      = (int)d.InfrastructureCostCurrency,
                opex          = d.AnnualOpex,
                opexCcy       = (int)d.AnnualOpexCurrency,
                ebm           = d.GridElectricityCost,
                ebmLow        = d.GridElectricityCostLow,
                ebmHigh       = d.GridElectricityCostHigh,
                touched       = true,
            }).ToList();

        var preload = new
        {
            feasibilityName = s.FeasibilityName,
            version         = s.Version,
            usdRate         = s.UsdRate,
            eurRate         = s.EurRate,
            inflTl          = s.InflationTl,
            inflUsd         = s.InflationUsd,
            inflEur         = s.InflationEur,
            vatRate         = s.VatRate,
            commissionRate  = s.CommissionRate,
            stations        = stationsJs,
        };

        return JsonSerializer.Serialize(preload, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    public async Task UpdateStudyAsync(Guid id, FeasibilityPricingSaveDto dto, CancellationToken ct = default)
    {
        var study = await _studyService.Query(ignoreFilters: true)
            .Where(s => s.Id == id)
            .Include(s => s.Stations)
            .FirstOrDefaultAsync(ct);

        if (study == null) return;

        foreach (var station in study.Stations)
            station.IsDeleted = true;

        _mapper.Map(dto, study);

        foreach (var stationDto in dto.Stations)
            study.Stations.Add(MapStation(stationDto, dto.UsdRate, dto.EurRate));

        await _studyService.SaveChangesAsync(ct);
    }

    public async Task<bool> IsLatestVersionAsync(Guid id, CancellationToken ct = default)
    {
        var study = await _studyService.Query(ignoreFilters: true)
            .Where(s => s.Id == id)
            .Select(s => new { s.FeasibilityName, s.Version })
            .FirstOrDefaultAsync(ct);

        if (study is null) return false;

        var maxVersion = await _studyService.Query()
            .Where(s => s.Kind == FeasibilityKind.Pricing && s.FeasibilityName == study.FeasibilityName)
            .Select(s => (int?)s.Version)
            .MaxAsync(ct) ?? study.Version;

        return study.Version >= maxVersion;
    }

    private PricingStation MapStation(PricingStationSaveDto stationDto, decimal usdRate, decimal eurRate)
    {
        var station = _mapper.Map<PricingStation>(stationDto);
        station.HardwareCostTl       = ToTl(stationDto.HardwareCost,       stationDto.HardwareCostCurrency,       usdRate, eurRate);
        station.InfrastructureCostTl = ToTl(stationDto.InfrastructureCost, stationDto.InfrastructureCostCurrency, usdRate, eurRate);
        station.AnnualOpexUsd        = ToUsd(stationDto.AnnualOpex,        stationDto.AnnualOpexCurrency,         usdRate, eurRate);
        // Ortalama EBM kullanıcıdan alınmaz; en düşük ile en yüksek EBM'nin ortalamasıdır (ana hesabın temeli).
        station.GridElectricityCost  = (stationDto.GridElectricityCostLow + stationDto.GridElectricityCostHigh) / 2m;
        return station;
    }

    private static FeasibilityPricingDetailDto BuildDetail(PricingStudy s)
    {
        var vat        = s.VatRate / 100m;        // KDV oranı (oran)
        var commission = s.CommissionRate / 100m; // Komisyon oranı (oran)

        var stations = s.Stations
            .Where(d => !d.IsDeleted)
            .OrderBy(d => d.DeviceType)
            .Select(d => BuildStationDetail(d, s.UsdRate, vat, commission))
            .ToList();

        return new FeasibilityPricingDetailDto
        {
            Id              = s.Id,
            FeasibilityName = s.FeasibilityName,
            Version         = s.Version,
            CreatedAt       = s.CreatedAt,
            CreatedByName   = s.CreatedByName,
            IsDeleted       = s.IsDeleted,
            UsdRate         = s.UsdRate,
            EurRate         = s.EurRate,
            InflationTl     = s.InflationTl,
            InflationUsd    = s.InflationUsd,
            InflationEur    = s.InflationEur,
            VatRate         = s.VatRate,
            CommissionRate  = s.CommissionRate,
            Stations        = stations,
        };
    }

    private static PricingStationDetailDto BuildStationDetail(PricingStation d, decimal usdRate, decimal vat, decimal commission)
    {
        var annualKwh = d.SocketCount * d.DailyKwhPerSocket * DaysPerYear;

        // CAPEX'in USD karşılığı (TL toplamından)
        var totalCapexTl  = d.HardwareCostTl + d.InfrastructureCostTl;
        var totalCapexUsd = usdRate > 0 ? totalCapexTl / usdRate : 0m;

        // Annüite faktörü: r / (1 - (1+r)^-n)
        var r = d.DiscountRate / 100m;
        var n = d.EconomicLifeYears;
        var annuityFactor = AnnuityFactor(r, n);

        // Yıllıklaştırılmış CAPEX (USD)
        var annualizedCapexUsd = totalCapexUsd * annuityFactor;

        // Birim maliyet kalemleri (USD/kWh)
        var depreciationShare = annualKwh > 0 ? annualizedCapexUsd / annualKwh : 0m;
        var opexShare         = annualKwh > 0 ? d.AnnualOpexUsd      / annualKwh : 0m;
        var energyShare       = d.GridElectricityCost; // ortalama EBM
        var baseCost          = depreciationShare + opexShare + energyShare;

        // Satış fiyatı (ana hesap, ortalama EBM)
        // Hedef kâr marjı NET SATIŞ MARJI olarak uygulanır: marj, maliyet üzerine
        // eklenen oran (markup) değil, satış fiyatı içindeki kâr payıdır.
        // Bu yüzden fiyat = maliyet / (1 - marj). Komisyon da satıştan kesinti
        // olduğundan aynı şekilde (1 - komisyon) ile bölünür.
        var margin = d.TargetProfitMargin / 100m;
        var oneMinusMargin = 1m - margin;
        var oneMinusCommission = 1m - commission;
        var denom = oneMinusMargin * oneMinusCommission;

        var saleExVat      = denom != 0 ? baseCost / denom : 0m;
        var recommended    = saleExVat * (1m + vat);

        // EBM senaryolu fiyatlar (KDV dahil)
        var fixedShare = depreciationShare + opexShare;
        decimal Scenario(decimal ebm)
            => denom != 0
                ? (fixedShare + ebm) / denom * (1m + vat)
                : 0m;

        return new PricingStationDetailDto
        {
            DeviceType                 = d.DeviceType.ToString(),
            SocketCount                = d.SocketCount,
            DailyKwhPerSocket          = d.DailyKwhPerSocket,
            AnnualSalesKwh             = annualKwh,
            EconomicLifeYears          = d.EconomicLifeYears,
            DiscountRate               = d.DiscountRate,
            TargetProfitMargin         = d.TargetProfitMargin,
            HardwareCost               = d.HardwareCost,
            HardwareCostCurrency       = d.HardwareCostCurrency,
            HardwareCostTl             = d.HardwareCostTl,
            InfrastructureCost         = d.InfrastructureCost,
            InfrastructureCostCurrency = d.InfrastructureCostCurrency,
            InfrastructureCostTl       = d.InfrastructureCostTl,
            TotalCapexTl               = totalCapexTl,

            AnnualOpex                 = d.AnnualOpex,
            AnnualOpexCurrency         = d.AnnualOpexCurrency,
            AnnualOpexUsd              = d.AnnualOpexUsd,

            GridElectricityCost        = d.GridElectricityCost,
            GridElectricityCostLow     = d.GridElectricityCostLow,
            GridElectricityCostHigh    = d.GridElectricityCostHigh,

            AnnuityFactor              = annuityFactor,
            AnnualizedCapexUsd         = annualizedCapexUsd,
            DepreciationShare          = depreciationShare,
            OpexShare                  = opexShare,
            EnergyShare                = energyShare,
            BaseCost                   = baseCost,

            SalePriceExVat             = saleExVat,
            RecommendedPrice           = recommended,
            SalePriceExVatTl           = saleExVat   * usdRate,
            RecommendedPriceTl         = recommended * usdRate,

            PriceLowEbm                = Scenario(d.GridElectricityCostLow),
            PriceAvgEbm                = Scenario(d.GridElectricityCost),
            PriceHighEbm               = Scenario(d.GridElectricityCostHigh),
        };
    }

    // Annüite faktörü: r / (1 - (1+r)^-n). r=0 ise 1/n (düz amortisman).
    private static decimal AnnuityFactor(decimal r, int n)
    {
        if (n <= 0) return 0m;
        if (r <= 0) return 1m / n;
        var denom = 1m - (decimal)Math.Pow((double)(1m + r), -n);
        return denom != 0 ? r / denom : 0m;
    }

    private static decimal ToTl(decimal amount, CurrencyType currency, decimal usdRate, decimal eurRate)
        => currency switch
        {
            CurrencyType.USD => amount * usdRate,
            CurrencyType.EUR => amount * eurRate,
            _                => amount
        };

    private static decimal ToUsd(decimal amount, CurrencyType currency, decimal usdRate, decimal eurRate)
        => currency switch
        {
            CurrencyType.USD => amount,
            CurrencyType.EUR => usdRate > 0 ? amount * eurRate / usdRate : 0m,
            _                => usdRate > 0 ? amount / usdRate          : 0m
        };
}

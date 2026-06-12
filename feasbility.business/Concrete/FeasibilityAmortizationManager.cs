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
    private readonly IWorldBankService           _worldBank;
    private readonly IEvdsInflationService       _evds;
    private readonly IMapper                     _mapper;

    public FeasibilityAmortizationManager(
        IGenericService<Location>   locationService,
        IGenericService<Study>      studyService,
        IGenericService<DeviceLine> deviceLineService,
        ITcmbService tcmb,
        IWorldBankService worldBank,
        IEvdsInflationService evds,
        IMapper mapper)
    {
        _locationService   = locationService;
        _studyService      = studyService;
        _deviceLineService = deviceLineService;
        _tcmb      = tcmb;
        _worldBank = worldBank;
        _evds      = evds;
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

    public async Task<Guid> SaveStudyAsync(FeasibilityAmortizationSaveDto dto, CancellationToken ct = default)
    {
        var study = _mapper.Map<Study>(dto);
        ApplyTlShadows(study, dto);

        study.Version = dto.SaveAsNewVersion ? dto.BaseVersion + 1 : 1;

        foreach (var lineDto in dto.DeviceLines)
        {
            var line = _mapper.Map<DeviceLine>(lineDto);
            line.UnitLocationCostTl = ToTl(lineDto.UnitLocationCost, lineDto.UnitLocationCostCurrency, dto.UsdRate, dto.EurRate);
            foreach (var projDto in lineDto.YearProjections)
                line.YearProjections.Add(_mapper.Map<YearProjection>(projDto));
            study.DeviceLines.Add(line);
        }

        await _studyService.AddAsync(study, ct);
        return study.Id;
    }

    public async Task<List<FeasibilityAmortizationListDto>> GetListAsync(CancellationToken ct = default)
    {
        var studies = await _studyService.Query(ignoreFilters: true)
            .Where(s => s.Kind == FeasibilityKind.Amortization)
            .Include(s => s.Location)
            .Include(s => s.DeviceLines)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

        var latestByName = studies
            .Where(s => !s.IsDeleted)
            .GroupBy(s => s.FeasibilityName)
            .ToDictionary(g => g.Key, g => g.Max(x => x.Version));

        return studies.Select(s =>
        {
            var oneTimeTl = s.StationUnitCostTl
                          + s.ProviderEntryFeeTl
                          + s.InfrastructureCostTl;

            var deviceTl = s.DeviceLines.Where(d => !d.IsDeleted).Sum(d =>
                  d.UnitLocationCostTl * d.DeviceCount);

            var totalTl  = oneTimeTl + deviceTl;
            var totalUsd = s.UsdRate > 0 ? totalTl / s.UsdRate : 0m;

            return new FeasibilityAmortizationListDto
            {
                Id                 = s.Id,
                FeasibilityName    = s.FeasibilityName,
                Version            = s.Version,
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
                IsLatest           = !latestByName.TryGetValue(s.FeasibilityName, out var maxV) || s.Version >= maxV,
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

        return BuildDetail(s);
    }

    public async Task<FeasibilityAmortizationDetailDto> CalculatePreviewAsync(FeasibilityAmortizationSaveDto dto, CancellationToken ct = default)
    {
        var study = _mapper.Map<Study>(dto);
        ApplyTlShadows(study, dto);

        foreach (var lineDto in dto.DeviceLines)
        {
            var line = _mapper.Map<DeviceLine>(lineDto);
            line.UnitLocationCostTl = ToTl(lineDto.UnitLocationCost, lineDto.UnitLocationCostCurrency, dto.UsdRate, dto.EurRate);
            foreach (var projDto in lineDto.YearProjections)
                line.YearProjections.Add(_mapper.Map<YearProjection>(projDto));
            study.DeviceLines.Add(line);
        }

        if (dto.LocationId != Guid.Empty)
            study.Location = await _locationService.GetByIdAsync(dto.LocationId, ignoreFilters: true, ct: ct);

        study.CreatedAt = DateTime.UtcNow;
        return BuildDetail(study);
    }

    private FeasibilityAmortizationDetailDto BuildDetail(Study s)
    {
        var oneTimeTl = s.StationUnitCostTl + s.ProviderEntryFeeTl + s.InfrastructureCostTl;
        var deviceTl  = s.DeviceLines.Where(d => !d.IsDeleted).Sum(d => d.UnitLocationCostTl * d.DeviceCount);
        var totalTl   = oneTimeTl + deviceTl;
        var totalUsd  = s.UsdRate > 0 ? totalTl / s.UsdRate : 0m;

        var annualRent        = s.HasRent ? s.MonthlyRentTl * 12 : 0m;
        var annualMaintenance = s.PostWarrantyMaintenanceCostTl;
        var annualAdvertising = s.AdvertisingRevenueTl;

        bool   hasLoan          = s.HasLoan && s.LoanAmount > 0 && s.LoanTermMonths > 0;
        decimal monthlyLoanPayment = 0, annualLoanPayment = 0;
        if (hasLoan)
        {
            var loanDays      = s.LoanTermMonths * 30m;
            var totalInterest = s.LoanAmount * s.LoanAnnualInterestRate * loanDays / 36000m;
            var totalBsm      = totalInterest * 0.05m;
            var totalRepay    = s.LoanAmount + totalInterest + totalBsm;
            monthlyLoanPayment = totalRepay / s.LoanTermMonths;
            annualLoanPayment  = monthlyLoanPayment * Math.Min(12, s.LoanTermMonths);
        }

        const decimal h1Days = 151m;
        const decimal h2Days = 214m;

        var uptimeFactor = 1m - Math.Clamp(s.MonthlyLostDaysPercent, 0m, 100m) / 100m;

        var activeLines = s.DeviceLines.Where(d => !d.IsDeleted).OrderBy(d => d.DeviceType).ToList();

        int baseProjYear = activeLines
            .SelectMany(d => d.YearProjections.Where(y => !y.IsDeleted).Select(y => y.Year))
            .DefaultIfEmpty(0)
            .Min();
        decimal ProjectedUsdRate(int year)
        {
            if (s.UsdRate <= 0) return s.UsdRate;
            var offset = baseProjYear > 0 ? year - baseProjYear : 0;
            if (offset <= 0) return s.UsdRate;
            var factor = 1m + s.InflationUsd / 100m;
            return s.UsdRate * (decimal)Math.Pow((double)factor, offset);
        }

        var linesDtos   = new List<DeviceLineDetailDto>();
        decimal totalRevTl = 0, totalElecTl = 0, totalCommTl = 0;

        var totalDeviceCount = activeLines.Sum(d => d.DeviceCount);
        var monthlyRentPerDevice = s.HasRent && totalDeviceCount > 0 ? s.MonthlyRentTl / totalDeviceCount : 0m;
        int[] daysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        foreach (var d in activeLines)
        {
            var lineInvestment = d.UnitLocationCostTl * d.DeviceCount;
            var y1 = d.YearProjections.Where(y => !y.IsDeleted).OrderBy(y => y.Year).FirstOrDefault();

            var (annualRev, annualElec) = ComputeLineYear(d, y1, h1Days, h2Days, uptimeFactor);

            var grossMargin = annualRev - annualElec;
            var commission  = d.AgreementGenre == Entity.Entities.Enums.AgreementGenre.Revenue
                ? d.AgreementRate * annualRev
                : d.AgreementRate * grossMargin;

            totalRevTl  += annualRev;
            totalElecTl += annualElec;
            totalCommTl += commission;

            var yearProjs = d.YearProjections
                .Where(y => !y.IsDeleted)
                .OrderBy(y => y.Year)
                .Select(y =>
                {
                    var (revTl, elecTl) = ComputeLineYear(d, y, h1Days, h2Days, uptimeFactor);
                    var gm   = revTl - elecTl;
                    var comm = d.AgreementGenre == Entity.Entities.Enums.AgreementGenre.Revenue
                        ? d.AgreementRate * revTl
                        : d.AgreementRate * gm;
                    var (eDaily, eSH1, eSH2, ePH1, ePH2) = EffectivePrices(d, y);
                    return new YearProjectionDetailDto
                    {
                        Year                    = y.Year,
                        DailyChargePerSocket    = eDaily,
                        SalePriceH1             = eSH1,
                        SalePriceH2             = eSH2,
                        PurchasePriceH1         = ePH1,
                        PurchasePriceH2         = ePH2,
                        UsdRate                 = ProjectedUsdRate(y.Year),
                        AnnualRevenueTl         = revTl,
                        AnnualElectricityCostTl = elecTl,
                        AnnualCommissionTl      = comm,
                    };
                }).ToList();

            var monthlyYears = d.YearProjections
                .Where(y => !y.IsDeleted)
                .OrderBy(y => y.Year)
                .Select(y =>
                {
                    var yUsd = ProjectedUsdRate(y.Year);
                    var (mDaily, mSH1, mSH2, mPH1, mPH2) = EffectivePrices(d, y);
                    var rows = new List<MonthlyBreakdownRowDto>();
                    for (int m = 1; m <= 12; m++)
                    {
                        var days       = daysPerMonth[m - 1];
                        var lostDays   = days * Math.Clamp(s.MonthlyLostDaysPercent, 0m, 100m) / 100m;
                        var netOpDays  = days - lostDays;
                        var isH1       = m <= 5;
                        var salePrice  = isH1 ? mSH1 : mSH2;
                        var buyPrice   = isH1 ? mPH1 : mPH2;

                        var saleKwh    = d.SocketCount * mDaily * d.AvgKwh * netOpDays;
                        var revTl      = saleKwh * salePrice;
                        var elecTl     = saleKwh * buyPrice;
                        var gmTl       = revTl - elecTl;
                        var commTl     = d.AgreementGenre == Entity.Entities.Enums.AgreementGenre.Revenue
                            ? d.AgreementRate * revTl
                            : d.AgreementRate * gmTl;
                        var rentTl     = monthlyRentPerDevice;
                        var grossTl    = revTl - commTl - rentTl - elecTl;

                        decimal toUsd(decimal tl) => yUsd > 0 ? tl / yUsd : 0m;

                        rows.Add(new MonthlyBreakdownRowDto
                        {
                            Month                 = m,
                            DaysInMonth           = days,
                            LostDays              = lostDays,
                            NetOperatingDays      = netOpDays,
                            SaleKwhPerDevice      = saleKwh,
                            SalePriceTlPerKwh     = salePrice,
                            PurchasePriceTlPerKwh = buyPrice,
                            RevenueTl             = revTl,
                            RevenueUsd            = toUsd(revTl),
                            CommissionTl          = commTl,
                            CommissionUsd         = toUsd(commTl),
                            RentTl                = rentTl,
                            RentUsd               = toUsd(rentTl),
                            ElectricityCostTl     = elecTl,
                            ElectricityCostUsd    = toUsd(elecTl),
                            GrossProfitTl         = grossTl,
                            GrossProfitUsd        = toUsd(grossTl),
                        });
                    }
                    return new MonthlyBreakdownYearDto
                    {
                        Year    = y.Year,
                        UsdRate = yUsd,
                        Months  = rows,
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
                MonthlyBreakdownYears   = monthlyYears,
            });
        }

        var annualFixedCosts = annualRent + annualMaintenance - annualAdvertising + annualLoanPayment;
        var annualNetTl      = totalRevTl - totalElecTl - totalCommTl - annualFixedCosts;
        var annualNetUsd     = s.UsdRate > 0 ? annualNetTl / s.UsdRate : 0m;
        var roi              = totalUsd > 0 ? Math.Round(annualNetUsd / totalUsd * 100, 1) : 0m;

        var allYears = activeLines
            .SelectMany(d => d.YearProjections.Where(y => !y.IsDeleted).Select(y => y.Year))
            .Distinct().OrderBy(y => y).ToList();

        int firstYear    = allYears.Count > 0 ? allYears[0] : 0;

        decimal LoanPaymentForYear(int year)
        {
            if (!hasLoan || firstYear == 0) return 0m;
            int yearIndex = year - firstYear;
            if (yearIndex < 0) return 0m;
            int monthsBefore = yearIndex * 12;
            if (monthsBefore >= s.LoanTermMonths) return 0m;
            int monthsThisYear = Math.Min(12, s.LoanTermMonths - monthsBefore);
            return monthlyLoanPayment * monthsThisYear;
        }

        var yearSummaries  = new List<YearSummaryDto>();
        decimal loanUsd    = hasLoan && s.UsdRate > 0 ? s.LoanAmount / s.UsdRate : 0m;
        decimal equityUsd  = Math.Round(totalUsd, 0) - Math.Round(loanUsd, 0);
        decimal cumUsd     = -equityUsd;

        foreach (var year in allYears)
        {
            decimal yRevTl = 0, yElecTl = 0, yCommTl = 0;
            decimal yUsdRate = ProjectedUsdRate(year);

            foreach (var d in activeLines)
            {
                var y = d.YearProjections.FirstOrDefault(p => p.Year == year && !p.IsDeleted);
                if (y == null) continue;

                var (rev, elec) = ComputeLineYear(d, y, h1Days, h2Days, uptimeFactor);
                var gm   = rev - elec;
                var comm = d.AgreementGenre == Entity.Entities.Enums.AgreementGenre.Revenue
                    ? d.AgreementRate * rev
                    : d.AgreementRate * gm;

                yRevTl  += rev;
                yElecTl += elec;
                yCommTl += comm;
            }

            var yLoanPmt   = LoanPaymentForYear(year);
            var yFixed     = annualRent + annualMaintenance - annualAdvertising + yLoanPmt;
            var yNetTl     = yRevTl - yElecTl - yCommTl - yFixed;
            var yNetUsd    = yUsdRate > 0 ? Math.Round(yNetTl / yUsdRate, 0) : 0m;
            cumUsd        += yNetUsd;

            yearSummaries.Add(new YearSummaryDto
            {
                Year                    = year,
                TotalRevenueTl          = Math.Round(yRevTl,  0),
                TotalElectricityCostTl  = Math.Round(yElecTl, 0),
                TotalCommissionTl       = Math.Round(yCommTl, 0),
                FixedCostsTl            = Math.Round(yFixed,  0),
                NetProfitTl             = Math.Round(yNetTl,  0),
                UsdRate                 = yUsdRate,
                NetProfitUsd            = yNetUsd,
                CumulativeBalanceUsd    = cumUsd,
            });
        }

        decimal payback = 0m;
        decimal prevCum = -equityUsd;
        for (int i = 0; i < yearSummaries.Count; i++)
        {
            var ys = yearSummaries[i];
            if (ys.CumulativeBalanceUsd >= 0)
            {
                var need = -prevCum;
                var frac = ys.NetProfitUsd > 0 ? need / ys.NetProfitUsd : 0m;
                payback = Math.Max(0.1m, Math.Round(i + frac, 1));
                break;
            }
            prevCum = ys.CumulativeBalanceUsd;
        }

        // Taşınabilir / geri kazanılabilir şarj istasyonu donanımı: hem üst formdaki tek seferlik
        // "İstasyon Birim Bedeli" hem de cihaz satırlarında girilen istasyon bedelleri (UnitLocationCost × adet).
        var stationTl       = s.StationUnitCostTl + deviceTl;
        var stationUsd      = s.UsdRate > 0 ? Math.Round(stationTl / s.UsdRate, 0) : 0m;
        var sunkUsd         = Math.Round(totalUsd, 0) - stationUsd;
        if (sunkUsd < 0) sunkUsd = 0m;

        var sunkRows        = new List<SunkAmortizationRowDto>();
        decimal sunkRecovered = 0m;
        bool sunkPaid         = false;
        decimal sunkPayback   = 0m;

        foreach (var ys in yearSummaries)
        {
            var prevRecovered = sunkRecovered;
            sunkRecovered += ys.NetProfitUsd;
            var remaining = sunkUsd - sunkRecovered;
            if (remaining < 0) remaining = 0m;

            bool isPaybackYear = !sunkPaid && sunkRecovered >= sunkUsd && sunkUsd > 0;
            if (isPaybackYear)
            {
                sunkPaid = true;
                var need = sunkUsd - prevRecovered;
                var frac = ys.NetProfitUsd > 0 ? need / ys.NetProfitUsd : 0m;
                sunkPayback = Math.Max(0.1m, Math.Round(sunkRows.Count + frac, 1));
            }

            sunkRows.Add(new SunkAmortizationRowDto
            {
                Year                   = ys.Year,
                NetProfitUsd           = ys.NetProfitUsd,
                RecoveredCumulativeUsd = Math.Round(sunkRecovered, 0),
                RemainingUsd           = Math.Round(remaining, 0),
                IsPaybackYear          = isPaybackYear,
            });
        }

        return new FeasibilityAmortizationDetailDto
        {
            Id                         = s.Id,
            FeasibilityName            = s.FeasibilityName,
            LocationName               = s.Location != null ? $"{s.Location.Name} — {s.Location.City} / {s.Location.District}" : string.Empty,
            CreatedAt                  = s.CreatedAt,
            CreatedByName              = s.CreatedByName,
            IsDeleted                  = s.IsDeleted,
            UsdRate                    = s.UsdRate,
            EurRate                    = s.EurRate,
            InflationTl                = s.InflationTl,
            InflationUsd               = s.InflationUsd,
            InflationEur               = s.InflationEur,
            ContractMonths             = s.ContractMonths,
            ContractStartDate          = s.ContractStartDate,
            MonthlyLostDaysPercent     = s.MonthlyLostDaysPercent,
            StationUnitCostTl          = s.StationUnitCostTl,
            ProviderEntryFeeTl         = s.ProviderEntryFeeTl,
            InfrastructureCostTl       = s.InfrastructureCostTl,
            DeviceTotalTl              = deviceTl,
            TotalInvestmentTl          = totalTl,
            TotalInvestmentUsd         = Math.Round(totalUsd, 0),
            EquityInvestmentUsd        = equityUsd,
            HasRent                    = s.HasRent,
            AnnualRentTl               = annualRent,
            AnnualMaintenanceTl        = annualMaintenance,
            AnnualAdvertisingRevenueTl = annualAdvertising,
            HasLoan                    = s.HasLoan,
            LoanAmount                 = s.LoanAmount,
            LoanAnnualInterestRate     = s.LoanAnnualInterestRate,
            LoanTermMonths             = s.LoanTermMonths,
            AnnualLoanPaymentTl        = annualLoanPayment,
            DeviceLines                = linesDtos,
            YearSummaries              = yearSummaries,
            AnnualNetProfitTl          = Math.Round(annualNetTl,  0),
            AnnualNetProfitUsd         = Math.Round(annualNetUsd, 0),
            PaybackYears               = payback,
            RoiPercent                 = roi,
            RecoverableInvestmentUsd   = stationUsd,
            SunkInvestmentUsd          = sunkUsd,
            SunkPaybackYears           = sunkPayback,
            SunkAmortization           = sunkRows,
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
                            .Select(y => new { year = y.Year, daily = y.DailyChargePerSocket, sH1 = y.SalePriceH1, sH2 = y.SalePriceH2, pH1 = y.PurchasePriceH1, pH2 = y.PurchasePriceH2, usd = y.UsdRate })
                            .ToList(),
                touched  = true,
            }).ToList();

        var preload = new
        {
            locationId               = s.LocationId.ToString(),
            feasibilityName          = s.FeasibilityName,
            version                  = s.Version,
            usdRate                  = s.UsdRate,
            eurRate                  = s.EurRate,
            inflTl                   = s.InflationTl,
            inflUsd                  = s.InflationUsd,
            inflEur                  = s.InflationEur,
            hasRent                  = s.HasRent,
            monthlyRent              = s.MonthlyRent,
            rentCurrency             = (int)s.RentCurrency,
            contractMonths           = s.ContractMonths,
            contractStartDate        = s.ContractStartDate.HasValue ? s.ContractStartDate.Value.ToString("yyyy-MM-dd") : null,
            monthlyLostDaysPercent   = s.MonthlyLostDaysPercent,
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

        _mapper.Map(dto, study);
        ApplyTlShadows(study, dto);

        foreach (var lineDto in dto.DeviceLines)
        {
            var line = _mapper.Map<DeviceLine>(lineDto);
            line.UnitLocationCostTl = ToTl(lineDto.UnitLocationCost, lineDto.UnitLocationCostCurrency, dto.UsdRate, dto.EurRate);
            foreach (var projDto in lineDto.YearProjections)
                line.YearProjections.Add(_mapper.Map<YearProjection>(projDto));
            study.DeviceLines.Add(line);
        }

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
            .Where(s => s.Kind == FeasibilityKind.Amortization && s.FeasibilityName == study.FeasibilityName)
            .Select(s => (int?)s.Version)
            .MaxAsync(ct) ?? study.Version;

        return study.Version >= maxVersion;
    }

    private static void ApplyTlShadows(Study study, FeasibilityAmortizationSaveDto dto)
    {
        study.MonthlyRentTl                   = ToTl(dto.MonthlyRent,                dto.RentCurrency,                    dto.UsdRate, dto.EurRate);
        study.PostWarrantyMaintenanceCostTl   = ToTl(dto.PostWarrantyMaintenanceCost, dto.PostWarrantyMaintenanceCurrency, dto.UsdRate, dto.EurRate);
        study.AdvertisingRevenueTl            = ToTl(dto.AdvertisingRevenue,          dto.AdvertisingRevenueCurrency,      dto.UsdRate, dto.EurRate);
        study.StationUnitCostTl               = ToTl(dto.StationUnitCost,             dto.StationUnitCostCurrency,         dto.UsdRate, dto.EurRate);
        study.ProviderEntryFeeTl              = ToTl(dto.ProviderEntryFee,            dto.ProviderEntryFeeCurrency,        dto.UsdRate, dto.EurRate);
        study.InfrastructureCostTl            = ToTl(dto.InfrastructureCost,          dto.InfrastructureCostCurrency,      dto.UsdRate, dto.EurRate);
        study.DeviceUnitCostTl                = ToTl(dto.DeviceUnitCost,              dto.DeviceUnitCostCurrency,          dto.UsdRate, dto.EurRate);
    }

    private static decimal ToTl(decimal amount, Entity.Entities.Enums.CurrencyType currency, decimal usdRate, decimal eurRate)
        => currency switch
        {
            Entity.Entities.Enums.CurrencyType.USD => amount * usdRate,
            Entity.Entities.Enums.CurrencyType.EUR => amount * eurRate,
            _                                       => amount
        };

    private static (decimal daily, decimal saleH1, decimal saleH2, decimal buyH1, decimal buyH2)
        EffectivePrices(DeviceLine d, YearProjection? y)
    {
        if (y == null)
            return (d.DailyChargesPerSocket, d.SalePriceTl, d.SalePriceTl, d.PurchasePriceTl, d.PurchasePriceTl);

        return (
            y.DailyChargePerSocket > 0 ? y.DailyChargePerSocket : d.DailyChargesPerSocket,
            y.SalePriceH1          > 0 ? y.SalePriceH1          : d.SalePriceTl,
            y.SalePriceH2          > 0 ? y.SalePriceH2          : d.SalePriceTl,
            y.PurchasePriceH1      > 0 ? y.PurchasePriceH1      : d.PurchasePriceTl,
            y.PurchasePriceH2      > 0 ? y.PurchasePriceH2      : d.PurchasePriceTl);
    }

    private static (decimal rev, decimal elec) ComputeLineYear(
        DeviceLine d, YearProjection? y, decimal h1Days, decimal h2Days, decimal uptimeFactor)
    {
        var (daily, sH1, sH2, pH1, pH2) = EffectivePrices(d, y);
        var eH1 = d.SocketCount * daily * h1Days * d.AvgKwh * uptimeFactor;
        var eH2 = d.SocketCount * daily * h2Days * d.AvgKwh * uptimeFactor;
        return (eH1 * sH1 + eH2 * sH2, eH1 * pH1 + eH2 * pH2);
    }
}

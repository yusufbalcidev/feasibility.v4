using feasibility.Entity.Dtos.FeasibilityAmortization;
using FluentValidation;

namespace feasibility.Business.Validators;

public class FeasibilityAmortizationSaveDtoValidator : AbstractValidator<FeasibilityAmortizationSaveDto>
{
    private const decimal MaxAmount = 100_000_000m;
    private const decimal MaxRate = 500m;

    public FeasibilityAmortizationSaveDtoValidator()
    {
        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("Lokasyon seçiniz.");

        RuleFor(x => x.FeasibilityName)
            .NotEmpty().WithMessage("Fizibilite adı zorunludur.")
            .MaximumLength(50).WithMessage("Fizibilite adı en fazla 50 karakter olabilir.");

        RuleFor(x => x.UsdRate)
            .GreaterThan(0).WithMessage("USD kuru 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxRate).WithMessage($"USD kuru en fazla {MaxRate:N0} olabilir.");
        RuleFor(x => x.EurRate)
            .GreaterThan(0).WithMessage("EUR kuru 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxRate).WithMessage($"EUR kuru en fazla {MaxRate:N0} olabilir.");

        RuleFor(x => x.InflationTl)
            .InclusiveBetween(0m, 100m).WithMessage("TL enflasyonu 0 ile 100 arasında olmalıdır.");
        RuleFor(x => x.InflationUsd)
            .InclusiveBetween(0m, 100m).WithMessage("USD enflasyonu 0 ile 100 arasında olmalıdır.");
        RuleFor(x => x.InflationEur)
            .InclusiveBetween(0m, 100m).WithMessage("EUR enflasyonu 0 ile 100 arasında olmalıdır.");

        When(x => x.HasRent, () =>
        {
            RuleFor(x => x.MonthlyRent)
                .InclusiveBetween(0m, MaxAmount).WithMessage($"Aylık kira 0 ile {MaxAmount:N0} arasında olmalıdır.");
        });

        RuleFor(x => x.ContractMonths)
            .GreaterThan(0).WithMessage("Sözleşme süresi zorunludur.")
            .LessThanOrEqualTo(600).WithMessage("Sözleşme süresi en fazla 600 ay olabilir.");

        RuleFor(x => x.MonthlyLostDaysPercent)
            .InclusiveBetween(0m, 99m).WithMessage("Aylık kayıp gün 0 ile 99 arasında olmalıdır.");

        RuleFor(x => x.PostWarrantyMaintenanceCost)
            .InclusiveBetween(0m, MaxAmount).WithMessage("Garanti sonrası bakım tutarı geçerli aralıkta olmalıdır.");
        RuleFor(x => x.AdvertisingRevenue)
            .InclusiveBetween(0m, MaxAmount).WithMessage("Reklam geliri geçerli aralıkta olmalıdır.");
        RuleFor(x => x.StationUnitCost)
            .InclusiveBetween(0m, MaxAmount).WithMessage("İstasyon birim maliyeti geçerli aralıkta olmalıdır.");
        RuleFor(x => x.ProviderEntryFee)
            .InclusiveBetween(0m, MaxAmount).WithMessage("Proje giriş bedeli geçerli aralıkta olmalıdır.");
        RuleFor(x => x.InfrastructureCost)
            .InclusiveBetween(0m, MaxAmount).WithMessage("Altyapı bedeli geçerli aralıkta olmalıdır.");
        RuleFor(x => x.DeviceUnitCost)
            .InclusiveBetween(0m, MaxAmount).WithMessage("Cihaz birim maliyeti geçerli aralıkta olmalıdır.");

        When(x => x.HasLoan, () =>
        {
            RuleFor(x => x.LoanAmount)
                .GreaterThan(0m).WithMessage("Kredi tutarı 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(MaxAmount).WithMessage($"Kredi tutarı en fazla {MaxAmount:N0} olabilir.");
            RuleFor(x => x.LoanAnnualInterestRate)
                .InclusiveBetween(0m, 100m).WithMessage("Yıllık faiz oranı 0 ile 100 arasında olmalıdır.");
            RuleFor(x => x.LoanTermMonths)
                .InclusiveBetween(1, 600).WithMessage("Vade 1 ile 600 ay arasında olmalıdır.");
        });

        RuleFor(x => x.DeviceLines)
            .NotEmpty().WithMessage("En az bir istasyon türü ekleyiniz.");

        RuleForEach(x => x.DeviceLines).SetValidator(new DeviceLineSaveDtoValidator());
    }
}

public class DeviceLineSaveDtoValidator : AbstractValidator<DeviceLineSaveDto>
{
    private const decimal MaxAmount = 100_000_000m;
    private const decimal MaxUnitPrice = 1_000m;

    public DeviceLineSaveDtoValidator()
    {
        RuleFor(x => x.DeviceCount)
            .InclusiveBetween(1, 1000).WithMessage("İstasyon adeti 1 ile 1.000 arasında olmalıdır.");
        RuleFor(x => x.SocketCount)
            .InclusiveBetween(1, 1000).WithMessage("Soket adeti 1 ile 1.000 arasında olmalıdır.");
        RuleFor(x => x.DailyChargesPerSocket)
            .GreaterThan(0m).WithMessage("Günlük soket başı şarjlanma 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(50m).WithMessage("Günlük soket başı şarjlanma en fazla 50 olabilir.");
        RuleFor(x => x.AvgKwh)
            .GreaterThan(0m).WithMessage("Ortalama şarjlanma (kWh) 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(500m).WithMessage("Ortalama şarjlanma (kWh) en fazla 500 olabilir.");
        RuleFor(x => x.SalePriceTl)
            .GreaterThan(0m).WithMessage("Satış fiyatı 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxUnitPrice).WithMessage($"Satış fiyatı en fazla {MaxUnitPrice:N0} TL/kWh olabilir.");
        RuleFor(x => x.PurchasePriceTl)
            .GreaterThan(0m).WithMessage("Alış fiyatı 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxUnitPrice).WithMessage($"Alış fiyatı en fazla {MaxUnitPrice:N0} TL/kWh olabilir.");
        RuleFor(x => x)
            .Must(x => x.PurchasePriceTl <= x.SalePriceTl)
            .WithMessage("Alış fiyatı, satış fiyatından yüksek olamaz.")
            .WithName(nameof(DeviceLineSaveDto.PurchasePriceTl));
        RuleFor(x => x.UnitLocationCost)
            .InclusiveBetween(0m, MaxAmount).WithMessage($"İstasyon bedeli 0 ile {MaxAmount:N0} arasında olmalıdır.");

        RuleFor(x => x.AgreementRate)
            .InclusiveBetween(0m, 0.99m).WithMessage("Sözleşme komisyon oranı %0 ile %99 arasında olmalıdır.");

        RuleForEach(x => x.YearProjections).SetValidator(new YearProjectionSaveDtoValidator());
        RuleFor(x => x)
            .Must(x => x.YearProjections.All(p =>
                (p.PurchasePriceH1 > 0 ? p.PurchasePriceH1 : x.PurchasePriceTl)
                <= (p.SalePriceH1 > 0 ? p.SalePriceH1 : x.SalePriceTl)))
            .WithMessage("Yıllık Ocak alış fiyatı, etkin Ocak satış fiyatından yüksek olamaz.")
            .WithName(nameof(DeviceLineSaveDto.YearProjections));
        RuleFor(x => x)
            .Must(x => x.YearProjections.All(p =>
                (p.PurchasePriceH2 > 0 ? p.PurchasePriceH2 : x.PurchasePriceTl)
                <= (p.SalePriceH2 > 0 ? p.SalePriceH2 : x.SalePriceTl)))
            .WithMessage("Yıllık Haziran alış fiyatı, etkin Haziran satış fiyatından yüksek olamaz.")
            .WithName(nameof(DeviceLineSaveDto.YearProjections));
    }
}

public class YearProjectionSaveDtoValidator : AbstractValidator<YearProjectionSaveDto>
{
    private const decimal MaxUnitPrice = 1_000m;
    private const decimal MaxRate = 500m;

    public YearProjectionSaveDtoValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100).WithMessage("Projeksiyon yılı 2000 ile 2100 arasında olmalıdır.");
        RuleFor(x => x.DailyChargePerSocket)
            .InclusiveBetween(0m, 50m).WithMessage("Günlük soket başı şarjlanma 0 ile 50 arasında olmalıdır.");
        RuleFor(x => x.SalePriceH1)
            .InclusiveBetween(0m, MaxUnitPrice).WithMessage($"Satış fiyatı (Ocak) en fazla {MaxUnitPrice:N0} TL/kWh olabilir.");
        RuleFor(x => x.SalePriceH2)
            .InclusiveBetween(0m, MaxUnitPrice).WithMessage($"Satış fiyatı (Haziran) en fazla {MaxUnitPrice:N0} TL/kWh olabilir.");
        RuleFor(x => x.PurchasePriceH1)
            .InclusiveBetween(0m, MaxUnitPrice).WithMessage($"Alış fiyatı (Ocak) en fazla {MaxUnitPrice:N0} TL/kWh olabilir.");
        RuleFor(x => x.PurchasePriceH2)
            .InclusiveBetween(0m, MaxUnitPrice).WithMessage($"Alış fiyatı (Haziran) en fazla {MaxUnitPrice:N0} TL/kWh olabilir.");
        RuleFor(x => x.UsdRate)
            .InclusiveBetween(0m, MaxRate).WithMessage($"USD kuru 0 ile {MaxRate:N0} arasında olmalıdır.");
    }
}

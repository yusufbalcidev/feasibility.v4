using feasibility.Entity.Dtos.FeasibilityPricing;
using FluentValidation;

namespace feasibility.Business.Validators;

public class FeasibilityPricingSaveDtoValidator : AbstractValidator<FeasibilityPricingSaveDto>
{
    private const decimal MaxRate = 500m;

    public FeasibilityPricingSaveDtoValidator()
    {
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

        RuleFor(x => x.VatRate)
            .InclusiveBetween(0m, 100m).WithMessage("KDV oranı %0 ile %100 arasında olmalıdır.");
        RuleFor(x => x.CommissionRate)
            .InclusiveBetween(0m, 99m).WithMessage("Komisyon oranı %0 ile %99 arasında olmalıdır.");

        RuleFor(x => x.Stations)
            .NotEmpty().WithMessage("En az bir istasyon ekleyiniz.");

        RuleForEach(x => x.Stations).SetValidator(new PricingStationSaveDtoValidator());
    }
}

public class PricingStationSaveDtoValidator : AbstractValidator<PricingStationSaveDto>
{
    private const decimal MaxAmount = 100_000_000m;

    public PricingStationSaveDtoValidator()
    {
        RuleFor(x => x.SocketCount)
            .InclusiveBetween(1, 1000).WithMessage("Soket adeti 1 ile 1.000 arasında olmalıdır.");

        RuleFor(x => x.DailyKwhPerSocket)
            .GreaterThan(0m).WithMessage("Soket başı günlük kullanım (kWh) 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(10_000m).WithMessage("Soket başı günlük kullanım (kWh) en fazla 10.000 olabilir.");

        RuleFor(x => x.EconomicLifeYears)
            .InclusiveBetween(1, 50).WithMessage("Ekonomik ömür 1 ile 50 yıl arasında olmalıdır.");

        RuleFor(x => x.DiscountRate)
            .InclusiveBetween(0m, 100m).WithMessage("İskonto oranı %0 ile %100 arasında olmalıdır.");

        RuleFor(x => x.TargetProfitMargin)
            .InclusiveBetween(10m, 16m).WithMessage("Hedef kâr marjı (ROI) %10 ile %16 arasında olmalıdır.");

        RuleFor(x => x.HardwareCost)
            .InclusiveBetween(0m, MaxAmount).WithMessage("Donanım maliyeti geçerli aralıkta olmalıdır.");
        RuleFor(x => x.InfrastructureCost)
            .InclusiveBetween(0m, MaxAmount).WithMessage("Altyapı kurulum maliyeti geçerli aralıkta olmalıdır.");

        RuleFor(x => x.AnnualOpex)
            .InclusiveBetween(0m, MaxAmount).WithMessage("Yıllık sabit OPEX geçerli aralıkta olmalıdır.");

        // Ortalama EBM kullanıcıdan alınmaz; (En düşük + En yüksek) / 2 olarak sunucuda hesaplanır.
        RuleFor(x => x.GridElectricityCostLow)
            .GreaterThan(0m).WithMessage("En düşük EBM 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxEbm).WithMessage($"En düşük EBM en fazla {MaxEbm:N2} USD/kWh olabilir.");
        RuleFor(x => x.GridElectricityCostHigh)
            .GreaterThan(0m).WithMessage("En yüksek EBM 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxEbm).WithMessage($"En yüksek EBM en fazla {MaxEbm:N2} USD/kWh olabilir.");
        RuleFor(x => x.GridElectricityCostHigh)
            .GreaterThanOrEqualTo(x => x.GridElectricityCostLow)
            .WithMessage("En yüksek EBM, en düşük EBM'den küçük olamaz.");
    }

    private const decimal MaxEbm = 100m;
}

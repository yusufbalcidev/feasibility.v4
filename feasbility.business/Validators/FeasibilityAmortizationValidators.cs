using feasibility.Entity.Dtos.FeasibilityAmortization;
using FluentValidation;

namespace feasibility.Business.Validators;

/// <summary>
/// Amortisman fizibilitesi kaydet/hesapla isteğinin sunucu tarafı doğrulaması.
/// İstemci (Create/Edit view'larındaki FizValidation) ile aynı iş sınırlarını uygular;
/// böylece istemci atlansa bile sınır dışı değerler hesabı patlatamaz.
///
/// Sınırlar (istemci ile birebir):
///   Yüzdeler: komisyon/kayıp gün 0–99, faiz 0–100, enflasyon 0–100 (oran olarak 0–1)
///   Kur: 0 (hariç) – 10.000
///   Tutarlar: 0 – 999.999.999 (fiyat/bedel pozitif)
///   Adet/soket: 1–9999, sözleşme/vade: 1–600 ay
/// </summary>
public class FeasibilityAmortizationSaveDtoValidator : AbstractValidator<FeasibilityAmortizationSaveDto>
{
    private const decimal MaxAmount = 999_999_999m;
    private const decimal MaxRate = 10_000m;

    public FeasibilityAmortizationSaveDtoValidator()
    {
        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("Lokasyon seçiniz.");

        RuleFor(x => x.FeasibilityName)
            .NotEmpty().WithMessage("Fizibilite adı zorunludur.")
            .MaximumLength(50).WithMessage("Fizibilite adı en fazla 50 karakter olabilir.");

        // Kurlar
        RuleFor(x => x.UsdRate)
            .GreaterThan(0).WithMessage("USD kuru 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxRate).WithMessage($"USD kuru en fazla {MaxRate:N0} olabilir.");
        RuleFor(x => x.EurRate)
            .GreaterThan(0).WithMessage("EUR kuru 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxRate).WithMessage($"EUR kuru en fazla {MaxRate:N0} olabilir.");

        // Enflasyonlar (yüzde olarak girilir: 0–100)
        RuleFor(x => x.InflationTl)
            .InclusiveBetween(0m, 100m).WithMessage("TL enflasyonu 0 ile 100 arasında olmalıdır.");
        RuleFor(x => x.InflationUsd)
            .InclusiveBetween(0m, 100m).WithMessage("USD enflasyonu 0 ile 100 arasında olmalıdır.");
        RuleFor(x => x.InflationEur)
            .InclusiveBetween(0m, 100m).WithMessage("EUR enflasyonu 0 ile 100 arasında olmalıdır.");

        // Kira (yalnızca kira varsa)
        When(x => x.HasRent, () =>
        {
            RuleFor(x => x.MonthlyRent)
                .InclusiveBetween(0m, MaxAmount).WithMessage($"Aylık kira 0 ile {MaxAmount:N0} arasında olmalıdır.");
        });

        // Sözleşme süresi zorunlu: 1–600 ay
        RuleFor(x => x.ContractMonths)
            .GreaterThan(0).WithMessage("Sözleşme süresi zorunludur.")
            .LessThanOrEqualTo(600).WithMessage("Sözleşme süresi en fazla 600 ay olabilir.");

        // Aylık kayıp gün yüzdesi: 0–99
        RuleFor(x => x.MonthlyLostDaysPercent)
            .InclusiveBetween(0m, 99m).WithMessage("Aylık kayıp gün 0 ile 99 arasında olmalıdır.");

        // Tutar alanları
        // Opsiyonel tutar alanları (boş/0 geçilebilir)
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

        // Kredi (yalnızca kredi varsa)
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

        // En az bir istasyon satırı
        RuleFor(x => x.DeviceLines)
            .NotEmpty().WithMessage("En az bir istasyon türü ekleyiniz.");

        RuleForEach(x => x.DeviceLines).SetValidator(new DeviceLineSaveDtoValidator());
    }
}

/// <summary>Tek bir istasyon (cihaz) satırının doğrulaması.</summary
public class DeviceLineSaveDtoValidator : AbstractValidator<DeviceLineSaveDto>
{
    private const decimal MaxAmount = 999_999_999m;

    public DeviceLineSaveDtoValidator()
    {
        RuleFor(x => x.DeviceCount)
            .InclusiveBetween(1, 9999).WithMessage("İstasyon adeti 1 ile 9999 arasında olmalıdır.");
        RuleFor(x => x.SocketCount)
            .InclusiveBetween(1, 9999).WithMessage("Soket adeti 1 ile 9999 arasında olmalıdır.");
        RuleFor(x => x.DailyChargesPerSocket)
            .GreaterThan(0m).WithMessage("Günlük soket başı şarjlanma 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(1000m).WithMessage("Günlük soket başı şarjlanma en fazla 1000 olabilir.");
        RuleFor(x => x.AvgKwh)
            .GreaterThan(0m).WithMessage("Ortalama şarjlanma (kWh) 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100000m).WithMessage("Ortalama şarjlanma (kWh) en fazla 100.000 olabilir.");
        RuleFor(x => x.SalePriceTl)
            .GreaterThan(0m).WithMessage("Satış fiyatı 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxAmount).WithMessage("Satış fiyatı geçerli aralıkta olmalıdır.");
        RuleFor(x => x.PurchasePriceTl)
            .GreaterThan(0m).WithMessage("Alış fiyatı 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(MaxAmount).WithMessage("Alış fiyatı geçerli aralıkta olmalıdır.");
        RuleFor(x => x.UnitLocationCost)
            .InclusiveBetween(0m, MaxAmount).WithMessage("İstasyon bedeli geçerli aralıkta olmalıdır.");

        // Komisyon oranı oran (0–1) olarak gönderilir: %0–99 => 0–0.99
        RuleFor(x => x.AgreementRate)
            .InclusiveBetween(0m, 0.99m).WithMessage("Sözleşme komisyon oranı %0 ile %99 arasında olmalıdır.");
    }
}

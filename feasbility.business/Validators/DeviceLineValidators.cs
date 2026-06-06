using feasibility.Entity.Dtos.DeviceLine;
using FluentValidation;

namespace feasibility.Business.Validators;

public class DeviceLineCreateDtoValidator : AbstractValidator<DeviceLineCreateDto>
{
    public DeviceLineCreateDtoValidator()
    {
        RuleFor(x => x.StudyId).NotEmpty().WithMessage("Fizibilite çalışması seçimi zorunludur.");

        RuleFor(x => x.DeviceCount)
            .GreaterThan(0).WithMessage("Cihaz adedi en az 1 olmalıdır.");

        RuleFor(x => x.SocketCount)
            .GreaterThan(0).WithMessage("Soket adedi en az 1 olmalıdır.");

        RuleFor(x => x.DailyChargesPerSocket).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalePriceTl).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PurchasePriceTl).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UnitLocationCost).GreaterThanOrEqualTo(0);

        RuleFor(x => x.AgreementRate)
            .InclusiveBetween(0m, 1m).WithMessage("Anlaşma oranı 0 ile 1 arasında olmalıdır.");
    }
}

public class DeviceLineUpdateDtoValidator : AbstractValidator<DeviceLineUpdateDto>
{
    public DeviceLineUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.StudyId).NotEmpty().WithMessage("Fizibilite çalışması seçimi zorunludur.");

        RuleFor(x => x.DeviceCount).GreaterThan(0).WithMessage("Cihaz adedi en az 1 olmalıdır.");
        RuleFor(x => x.SocketCount).GreaterThan(0).WithMessage("Soket adedi en az 1 olmalıdır.");

        RuleFor(x => x.DailyChargesPerSocket).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalePriceTl).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PurchasePriceTl).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UnitLocationCost).GreaterThanOrEqualTo(0);

        RuleFor(x => x.AgreementRate)
            .InclusiveBetween(0m, 1m).WithMessage("Anlaşma oranı 0 ile 1 arasında olmalıdır.");
    }
}

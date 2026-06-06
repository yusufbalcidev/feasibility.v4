using feasibility.Entity.Dtos.YearProjection;
using FluentValidation;

namespace feasibility.Business.Validators;

public class YearProjectionCreateDtoValidator : AbstractValidator<YearProjectionCreateDto>
{
    public YearProjectionCreateDtoValidator()
    {
        RuleFor(x => x.DeviceLineId).NotEmpty().WithMessage("Cihaz satırı seçimi zorunludur.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1, 50).WithMessage("Yıl 1 ile 50 arasında olmalıdır.");

        RuleFor(x => x.DailyChargePerSocket)
            .GreaterThanOrEqualTo(0).WithMessage("Günlük şarj tutarı negatif olamaz.");
    }
}

public class YearProjectionUpdateDtoValidator : AbstractValidator<YearProjectionUpdateDto>
{
    public YearProjectionUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DeviceLineId).NotEmpty().WithMessage("Cihaz satırı seçimi zorunludur.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1, 50).WithMessage("Yıl 1 ile 50 arasında olmalıdır.");

        RuleFor(x => x.DailyChargePerSocket)
            .GreaterThanOrEqualTo(0).WithMessage("Günlük şarj tutarı negatif olamaz.");
    }
}

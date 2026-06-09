using feasibility.Entity.Dtos.Location;
using FluentValidation;

namespace feasibility.Business.Validators;

public class LocationCreateDtoValidator : AbstractValidator<LocationCreateDto>
{
    public LocationCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Lokasyon adı zorunludur.")
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("İl seçimi zorunludur.")
            .MaximumLength(100);

        RuleFor(x => x.District)
            .NotEmpty().WithMessage("İlçe seçimi zorunludur.")
            .MaximumLength(100);

        RuleFor(x => x.Address).MaximumLength(300);

        RuleFor(x => x.Latitude)
            .NotNull().WithMessage("Enlem zorunludur.");
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m).WithMessage("Enlem -90 ile 90 arasında olmalıdır.")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .NotNull().WithMessage("Boylam zorunludur.");
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m).WithMessage("Boylam -180 ile 180 arasında olmalıdır.")
            .When(x => x.Longitude.HasValue);
    }
}

public class LocationUpdateDtoValidator : AbstractValidator<LocationUpdateDto>
{
    public LocationUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.District).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address).MaximumLength(300);
        RuleFor(x => x.Latitude)
            .NotNull().WithMessage("Enlem zorunludur.");
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m).WithMessage("Enlem -90 ile 90 arasında olmalıdır.")
            .When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude)
            .NotNull().WithMessage("Boylam zorunludur.");
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m).WithMessage("Boylam -180 ile 180 arasında olmalıdır.")
            .When(x => x.Longitude.HasValue);
    }
}

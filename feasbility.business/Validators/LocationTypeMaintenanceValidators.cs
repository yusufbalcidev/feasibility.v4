using feasibility.Entity.Dtos.LocationTypeMaintenance;
using FluentValidation;

namespace feasibility.Business.Validators;

public class LocationTypeMaintenanceCreateDtoValidator : AbstractValidator<LocationTypeMaintenanceCreateDto>
{
    public LocationTypeMaintenanceCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tip adı zorunludur.")
            .MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class LocationTypeMaintenanceUpdateDtoValidator : AbstractValidator<LocationTypeMaintenanceUpdateDto>
{
    public LocationTypeMaintenanceUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

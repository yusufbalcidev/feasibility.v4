using feasibility.Entity.Dtos.Role;
using FluentValidation;

namespace feasibility.Business.Validators;

public class RoleCreateDtoValidator : AbstractValidator<RoleCreateDto>
{
    public RoleCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Rol adı zorunludur.").MaximumLength(80);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class RoleUpdateDtoValidator : AbstractValidator<RoleUpdateDto>
{
    public RoleUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

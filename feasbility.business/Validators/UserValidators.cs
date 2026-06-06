using feasibility.Entity.Dtos.User;
using FluentValidation;

namespace feasibility.Business.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad zorunludur.").MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad zorunludur.").MaximumLength(50);
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı zorunludur.").MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta zorunludur.").EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
        RuleFor(x => x.PasswordConfirm)
            .Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor.");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Rol seçimi zorunludur.");
    }
}

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Rol seçimi zorunludur.");
        When(x => !string.IsNullOrWhiteSpace(x.NewPassword), () =>
        {
            RuleFor(x => x.NewPassword!).MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
        });
    }
}

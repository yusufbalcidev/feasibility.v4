using feasibility.Entity.Dtos.Auth;
using FluentValidation;

namespace feasibility.Business.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı zorunludur.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre zorunludur.");
    }
}

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta zorunludur.").EmailAddress();
    }
}

public class VerifyCodeDtoValidator : AbstractValidator<VerifyCodeDto>
{
    public VerifyCodeDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty().Length(6).WithMessage("Doğrulama kodu 6 haneli olmalıdır.");
    }
}

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
        RuleFor(x => x.NewPasswordConfirm)
            .Equal(x => x.NewPassword).WithMessage("Şifreler eşleşmiyor.");
    }
}

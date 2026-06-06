using System.ComponentModel.DataAnnotations;

namespace feasibility.Entity.Dtos.Auth;

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
    public string Email { get; set; } = string.Empty;
}

public class VerifyCodeDto
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kod zorunludur.")]
    public string Code { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Şifreler eşleşmiyor.")]
    public string NewPasswordConfirm { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace feasibility.Entity.Dtos.User;

public class UserCreateDto
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
    public string PasswordConfirm { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rol seçimi zorunludur.")]
    public Guid RoleId { get; set; }
}

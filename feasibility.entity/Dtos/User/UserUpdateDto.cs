using System.ComponentModel.DataAnnotations;

namespace feasibility.Entity.Dtos.User;

public class UserUpdateDto
{
    public Guid Id { get; set; }

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
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Rol seçimi zorunludur.")]
    public Guid RoleId { get; set; }
}

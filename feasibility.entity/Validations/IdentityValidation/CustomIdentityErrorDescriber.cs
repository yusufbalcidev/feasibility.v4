using Microsoft.AspNetCore.Identity;

namespace feasibility.Entity.Validations.IdentityValidation;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateUserName(string? userName) =>
        new() { Code = "DuplicateUserName", Description = $"\"{userName}\" kullanıcı adı kullanılmaktadır." };

    public override IdentityError InvalidUserName(string? userName) =>
        new() { Code = "InvalidUserName", Description = "Geçersiz kullanıcı adı." };

    public override IdentityError DuplicateEmail(string? email) =>
        new() { Code = "DuplicateEmail", Description = $"\"{email}\" başka bir kullanıcı tarafından kullanılmaktadır." };

    public override IdentityError InvalidEmail(string? email) =>
        new() { Code = "InvalidEmail", Description = "Geçersiz e-posta." };

    public override IdentityError PasswordTooShort(int length) =>
        new() { Code = "PasswordTooShort", Description = $"Şifre en az {length} karakter olmalıdır." };

    public override IdentityError PasswordMismatch() =>
        new() { Code = "PasswordMismatch", Description = "Şifre yanlış." };
}

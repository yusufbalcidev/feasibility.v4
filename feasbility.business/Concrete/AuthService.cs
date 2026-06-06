using System.Collections.Concurrent;
using feasibility.Business.Abstract;
using feasibility.Entity.Dtos.Auth;
using feasibility.Entity.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace feasibility.Business.Concrete;

public class AuthService : IAuthService
{
    private static readonly ConcurrentDictionary<string, ResetCodeEntry> _resetCodes = new();
    private static readonly ConcurrentDictionary<string, string> _verifiedTokens = new();
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(10);

    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtService jwtService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user is null || user.IsDeleted)
            return Fail("Kullanıcı adı veya şifre hatalı.");

        if (await _userManager.IsLockedOutAsync(user))
        {
            var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
            var remaining = lockoutEnd!.Value - DateTimeOffset.UtcNow;
            return Fail($"Hesap kilitlendi. {Math.Max(1, (int)remaining.TotalMinutes)} dakika sonra tekrar deneyin.");
        }

        var passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordOk)
        {
            await _userManager.AccessFailedAsync(user);
            if (await _userManager.IsLockedOutAsync(user))
                return Fail("5 hatalı denemeden sonra hesabınız 1 saat kilitlendi.");

            var failed = await _userManager.GetAccessFailedCountAsync(user);
            return Fail($"Kullanıcı adı veya şifre hatalı. Kalan deneme: {5 - failed}.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtService.GenerateToken(user, roles);

        return new AuthResultDto
        {
            Succeeded = true,
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            UserName = user.UserName,
            RoleName = roles.FirstOrDefault()
        };
    }

    public async Task<AuthResultDto> SendResetCodeAsync(ForgotPasswordDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || user.IsDeleted)
            return Fail("Bu e-posta adresine kayıtlı bir kullanıcı bulunamadı.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var code = Random.Shared.Next(100000, 999999).ToString();
        _resetCodes[dto.Email] = new ResetCodeEntry(code, DateTime.UtcNow.Add(CodeLifetime), token);

        await _emailService.SendResetCodeAsync(user.Email!, code, ct);
        return new AuthResultDto { Succeeded = true, Message = "Doğrulama kodu e-posta adresinize gönderildi." };
    }

    public Task<AuthResultDto> VerifyCodeAsync(VerifyCodeDto dto, CancellationToken ct = default)
    {
        if (!_resetCodes.TryGetValue(dto.Email, out var entry))
            return Task.FromResult(Fail("Kod bulunamadı. Lütfen tekrar talep edin."));

        if (DateTime.UtcNow > entry.Expiry)
        {
            _resetCodes.TryRemove(dto.Email, out _);
            return Task.FromResult(Fail("Kodun süresi doldu. Lütfen tekrar talep edin."));
        }

        if (!string.Equals(entry.Code, dto.Code.Trim(), StringComparison.Ordinal))
            return Task.FromResult(Fail("Girdiğiniz kod hatalı."));

        _resetCodes.TryRemove(dto.Email, out _);
        _verifiedTokens[dto.Email] = entry.Token;
        ScheduleTokenExpiry(dto.Email);
        return Task.FromResult(new AuthResultDto { Succeeded = true, Token = entry.Token });
    }

    public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || user.IsDeleted)
            return Fail("Kullanıcı bulunamadı.");

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded)
            return Fail(string.Join(" ", result.Errors.Select(e => e.Description)));

        _verifiedTokens.TryRemove(dto.Email, out _);
        await _userManager.ResetAccessFailedCountAsync(user);
        return new AuthResultDto { Succeeded = true, Message = "Şifreniz başarıyla güncellendi." };
    }

    public async Task<AuthResultDto> ResendResetCodeAsync(string email, CancellationToken ct = default) =>
        await SendResetCodeAsync(new ForgotPasswordDto { Email = email }, ct);

    public bool TryGetPendingResetToken(string email, out string token)
    {
        if (_verifiedTokens.TryGetValue(email, out var t))
        {
            token = t;
            return true;
        }
        token = string.Empty;
        return false;
    }

    private static AuthResultDto Fail(string message) => new() { Succeeded = false, Message = message };

    private static void ScheduleTokenExpiry(string email)
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(TokenLifetime);
            _verifiedTokens.TryRemove(email, out _);
        });
    }

    private record ResetCodeEntry(string Code, DateTime Expiry, string Token);
}

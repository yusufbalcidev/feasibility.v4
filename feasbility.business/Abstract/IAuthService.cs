using feasibility.Entity.Dtos.Auth;
using Microsoft.AspNetCore.Identity;

namespace feasibility.Business.Abstract;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<AuthResultDto> SendResetCodeAsync(ForgotPasswordDto dto, CancellationToken ct = default);
    Task<AuthResultDto> VerifyCodeAsync(VerifyCodeDto dto, CancellationToken ct = default);
    Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default);
    Task<AuthResultDto> ResendResetCodeAsync(string email, CancellationToken ct = default);
    bool TryGetPendingResetToken(string email, out string token);
}

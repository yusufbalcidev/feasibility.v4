using feasibility.Business.Abstract;
using feasibility.Entity.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace feasibility.App.Controllers;

[AllowAnonymous]
public class LoginController : Controller
{
    private const string TokenCookieName = "feasibility_jwt";

    private readonly IAuthService _authService;

    public LoginController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (User?.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _authService.LoginAsync(dto);
        if (!result.Succeeded)
        {
            TempData["Error"] = result.Message;
            return View(dto);
        }

        Response.Cookies.Append(TokenCookieName, result.Token!, BuildCookieOptions(result.ExpiresAt!.Value));
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(TokenCookieName);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _authService.SendResetCodeAsync(dto);
        if (!result.Succeeded)
        {
            TempData["Error"] = result.Message;
            return View(dto);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(VerifyCode), new { email = dto.Email });
    }

    [HttpGet]
    public IActionResult VerifyCode(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return RedirectToAction(nameof(ForgotPassword));
        return View(new VerifyCodeDto { Email = email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyCode(VerifyCodeDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _authService.VerifyCodeAsync(dto);
        if (!result.Succeeded)
        {
            TempData["Error"] = result.Message;
            return View(dto);
        }

        return RedirectToAction(nameof(ResetPassword), new { email = dto.Email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendCode(string email)
    {
        var result = await _authService.ResendResetCodeAsync(email);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(VerifyCode), new { email });
    }

    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !_authService.TryGetPendingResetToken(email, out var token))
            return RedirectToAction(nameof(ForgotPassword));

        return View(new ResetPasswordDto { Email = email, Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _authService.ResetPasswordAsync(dto);
        if (!result.Succeeded)
        {
            TempData["Error"] = result.Message;
            return View(dto);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private static CookieOptions BuildCookieOptions(DateTime expiresAt) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = expiresAt
    };
}

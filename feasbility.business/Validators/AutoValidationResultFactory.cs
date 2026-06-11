using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace feasibility.Business.Validators;

/// <summary>
/// AutoValidation doğrulama hatasında dönecek 400 yanıtını biçimlendirir.
/// İstemci (fetch) tarafı <c>JSON.parse(t).message</c> okuduğu için yanıtı
/// { message, errors } şeklinde döndürürüz; ilk hata mesajı kullanıcıya gösterilir.
/// </summary>
public class AutoValidationResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(
        ActionExecutingContext context,
        ValidationProblemDetails? validationProblemDetails)
    {
        var modelState = context.ModelState;

        var errors = modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        var firstMessage = modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m)) ?? "Geçersiz veri.";

        return new BadRequestObjectResult(new { message = firstMessage, errors });
    }
}

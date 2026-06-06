using System.Security.Claims;
using feasibility.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace feasibility.Business.Middlewares;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class PagePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    public string PageKey { get; }
    public string Action { get; }

    public PagePermissionAttribute(string pageKey, string action = "view")
    {
        PageKey = pageKey;
        Action = action;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        if (user.IsInRole("SuperAdmin")) return;

        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var service = context.HttpContext.RequestServices.GetRequiredService<IRolePermissionService>();
        var allowed = await service.UserHasPagePermissionAsync(userId, PageKey, Action);
        if (!allowed)
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
    }
}

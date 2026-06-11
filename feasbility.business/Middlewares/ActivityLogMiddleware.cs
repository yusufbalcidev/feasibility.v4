using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using feasibility.Business.Abstract;
using feasibility.Entity.Entities.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace feasibility.Business.Middlewares;

public class ActivityLogMiddleware
{
    private const int MaxBodyChars = 8000;

    private static readonly string[] IgnoredPathPrefixes =
    {
        "/css", "/js", "/lib", "/images", "/favicon.ico", "/data"
    };

    private static readonly string[] SensitivePathPrefixes =
    {
        "/login", "/account"
    };

    private static readonly string[] LoggableContentTypes =
    {
        "application/json",
        "application/x-www-form-urlencoded",
        "multipart/form-data",
        "text/plain",
        "text/json"
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ActivityLogMiddleware> _logger;

    public ActivityLogMiddleware(RequestDelegate next, ILogger<ActivityLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopeFactory)
    {
        if (ShouldSkipEntirely(context))
        {
            await _next(context);
            return;
        }

        if (!ShouldLog(context))
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();
        Exception? capturedException = null;

        bool isSensitive = IsSensitivePath(context.Request.Path);
        string? requestBody = null;
        string? responseBody = null;

        if (!isSensitive && CanBufferRequest(context.Request))
        {
            requestBody = await ReadRequestBodyAsync(context);
        }

        var originalResponseBody = context.Response.Body;
        await using var responseBuffer = new MemoryStream();
        context.Response.Body = responseBuffer;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            capturedException = ex;
            throw;
        }
        finally
        {
            sw.Stop();
            try
            {
                if (!isSensitive && CanCaptureResponse(context.Response))
                {
                    responseBody = ReadResponseBuffer(responseBuffer);
                }

                responseBuffer.Position = 0;
                await responseBuffer.CopyToAsync(originalResponseBody);
                context.Response.Body = originalResponseBody;

                using var scope = scopeFactory.CreateScope();
                var logService = scope.ServiceProvider.GetRequiredService<IActivityLogService>();
                var entry = BuildLogEntry(context, sw.ElapsedMilliseconds, capturedException, requestBody, responseBody);
                await logService.LogAsync(entry, context.RequestAborted);
            }
            catch (Exception loggingException)
            {
                _logger.LogError(loggingException, "Activity log middleware failed to persist log entry");
            }
        }
    }

    private static bool ShouldSkipEntirely(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        return IgnoredPathPrefixes.Any(prefix =>
            path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    private static bool ShouldLog(HttpContext context)
    {
        return !HttpMethods.IsGet(context.Request.Method) &&
               !HttpMethods.IsHead(context.Request.Method) &&
               !HttpMethods.IsOptions(context.Request.Method);
    }

    private static bool IsSensitivePath(PathString path)
    {
        var pathValue = path.Value ?? string.Empty;
        return SensitivePathPrefixes.Any(prefix =>
            pathValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    private static bool CanBufferRequest(HttpRequest request)
    {
        if (request.ContentLength is null or 0) return false;
        if (request.ContentLength > 50_000) return false;
        var ct = request.ContentType?.ToLowerInvariant() ?? string.Empty;
        return LoggableContentTypes.Any(t => ct.StartsWith(t));
    }

    private static bool CanCaptureResponse(HttpResponse response)
    {
        var ct = response.ContentType?.ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrEmpty(ct)) return false;
        return ct.StartsWith("application/json") ||
               ct.StartsWith("text/plain") ||
               ct.StartsWith("text/json") ||
               ct.StartsWith("application/problem+json");
    }

    private static async Task<string?> ReadRequestBodyAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        return TrimBody(MaskSensitiveFields(body, context.Request.ContentType));
    }

    private static string ReadResponseBuffer(MemoryStream buffer)
    {
        buffer.Position = 0;
        using var reader = new StreamReader(buffer, Encoding.UTF8, leaveOpen: true);
        var content = reader.ReadToEnd();
        buffer.Position = 0;
        return TrimBody(content);
    }

    private static string TrimBody(string? body)
    {
        if (string.IsNullOrEmpty(body)) return string.Empty;
        return body.Length <= MaxBodyChars ? body : body[..MaxBodyChars] + "…";
    }

    private static string MaskSensitiveFields(string body, string? contentType)
    {
        if (string.IsNullOrEmpty(body)) return body;
        var lowered = (contentType ?? string.Empty).ToLowerInvariant();
        if (!lowered.StartsWith("application/x-www-form-urlencoded")) return body;

        var pairs = QueryHelpers.ParseQuery(body);
        if (pairs.Count == 0) return body;

        var sb = new StringBuilder();
        foreach (var kv in pairs)
        {
            if (sb.Length > 0) sb.Append('&');
            var value = IsSensitiveKey(kv.Key) ? "***" : kv.Value.ToString();
            sb.Append(Uri.EscapeDataString(kv.Key)).Append('=').Append(Uri.EscapeDataString(value));
        }
        return sb.ToString();
    }

    private static bool IsSensitiveKey(string key) =>
        key.Contains("password", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("token", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("secret", StringComparison.OrdinalIgnoreCase);

    private static ActivityLog BuildLogEntry(
        HttpContext context,
        long elapsedMs,
        Exception? exception,
        string? requestBody,
        string? responseBody)
    {
        var principal = context.User;
        Guid? userId = null;
        if (Guid.TryParse(principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var parsed))
            userId = parsed;

        var routeData = context.GetRouteData();
        var controller = routeData?.Values["controller"]?.ToString();
        var action = routeData?.Values["action"]?.ToString();

        var method = context.Request.Method;
        var statusCode = context.Response?.StatusCode ?? 0;

        var actionType = (method.ToUpperInvariant(), action) switch
        {
            ("POST", { } a) when a.Equals("Delete", StringComparison.OrdinalIgnoreCase) => ActivityActionType.Delete,
            ("POST", { } a) when a.Equals("Create", StringComparison.OrdinalIgnoreCase) => ActivityActionType.Create,
            ("POST", { } a) when a.Equals("Edit", StringComparison.OrdinalIgnoreCase) => ActivityActionType.Update,
            ("POST", { } a) when a.Equals("Logout", StringComparison.OrdinalIgnoreCase) => ActivityActionType.Logout,
            _ => ActivityActionType.Update
        };

        if (controller?.Equals("Login", StringComparison.OrdinalIgnoreCase) == true)
        {
            actionType = action switch
            {
                "Index" => ActivityActionType.Login,
                "Logout" => ActivityActionType.Logout,
                "ForgotPassword" or "VerifyCode" or "ResetPassword" or "ResendCode" => ActivityActionType.PasswordReset,
                _ => actionType
            };
        }

        if (exception != null || statusCode >= 500)
            actionType = ActivityActionType.Error;

        Guid? studyId = null;
        string? studyName = null;
        if (controller?.Equals("FeasibilityAmortization", StringComparison.OrdinalIgnoreCase) == true)
        {
            if (routeData?.Values["id"]?.ToString() is { } routeIdStr &&
                Guid.TryParse(routeIdStr, out var routeGuid))
            {
                studyId = routeGuid;
            }
            if (studyId is null &&
                context.Request.Query.TryGetValue("id", out var qId) &&
                Guid.TryParse(qId, out var queryGuid))
            {
                studyId = queryGuid;
            }

            studyName = ExtractFeasibilityName(requestBody);

            if (studyId is null && !string.IsNullOrEmpty(responseBody))
            {
                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("id", out var idEl) &&
                        idEl.TryGetGuid(out var respGuid))
                        studyId = respGuid;
                }
                catch { }
            }
        }

        return new ActivityLog
        {
            UserId = userId,
            UserName = principal?.Identity?.Name,
            UserEmail = principal?.FindFirstValue(ClaimTypes.Email),
            HttpMethod = method,
            Path = context.Request.Path.ToString(),
            QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
            Controller = controller,
            Action = action,
            StatusCode = statusCode,
            ElapsedMs = elapsedMs,
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers.UserAgent.ToString(),
            ActionType = actionType,
            Description = BuildDescription(controller, action, method),
            ErrorMessage = exception?.Message,
            StudyId = studyId,
            StudyName = studyName,
            RequestContentType = context.Request.ContentType,
            RequestBody = requestBody,
            ResponseContentType = context.Response?.ContentType,
            ResponseBody = responseBody
        };
    }

    private static string? ExtractFeasibilityName(string? requestBody)
    {
        if (string.IsNullOrEmpty(requestBody)) return null;
        try
        {
            using var doc = JsonDocument.Parse(requestBody);
            foreach (var candidate in new[] { "feasibilityName", "FeasibilityName" })
            {
                if (doc.RootElement.TryGetProperty(candidate, out var el) &&
                    el.ValueKind == JsonValueKind.String)
                    return el.GetString();
            }
        }
        catch { }
        return null;
    }

    private static string BuildDescription(string? controller, string? action, string method)
    {
        if (string.IsNullOrEmpty(controller)) return $"{method} request";
        return $"{controller}/{action} [{method}]";
    }
}

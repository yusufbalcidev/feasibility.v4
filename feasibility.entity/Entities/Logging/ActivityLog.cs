using feasibility.Entity.Entities.Common;

namespace feasibility.Entity.Entities.Logging;

public class ActivityLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }

    public string HttpMethod { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public string? Controller { get; set; }
    public string? Action { get; set; }

    public int StatusCode { get; set; }
    public long ElapsedMs { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public string ActionType { get; set; } = ActivityActionType.Read;
    public string? Description { get; set; }
    public string? ErrorMessage { get; set; }

    public Guid? StudyId { get; set; }
    public string? StudyName { get; set; }

    public string? RequestContentType { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseContentType { get; set; }
    public string? ResponseBody { get; set; }
}

public static class ActivityActionType
{
    public const string Read = "Read";
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";
    public const string Login = "Login";
    public const string Logout = "Logout";
    public const string LoginFailed = "LoginFailed";
    public const string PasswordReset = "PasswordReset";
    public const string Error = "Error";
}

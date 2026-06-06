namespace feasibility.Entity.Dtos.ActivityLog;

public class ActivityLogDetailDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
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
    public string ActionType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RequestContentType { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseContentType { get; set; }
    public string? ResponseBody { get; set; }
}

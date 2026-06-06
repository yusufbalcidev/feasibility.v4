namespace feasibility.Entity.Dtos.Auth;

public class AuthResultDto
{
    public bool Succeeded { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Message { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? RoleName { get; set; }
}

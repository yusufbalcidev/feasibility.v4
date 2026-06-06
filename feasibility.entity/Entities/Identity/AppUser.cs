using Microsoft.AspNetCore.Identity;

namespace feasibility.Entity.Entities.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public string? DeletedByName { get; set; }
}

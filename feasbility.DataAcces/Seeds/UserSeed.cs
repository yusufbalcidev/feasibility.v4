using feasibility.Entity.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace feasibility.DataAccess.Seeds;

public static class UserSeed
{
    public static readonly Guid SuperAdminUserId = new("33333333-3333-3333-3333-000000000001");

    public const string SuperAdminUserName = "superadmin";
    public const string SuperAdminEmail = "superadmin@ekaenerji.com.tr";
    public const string SuperAdminFirstName = "Süper";
    public const string SuperAdminLastName = "Admin";
    public const string DefaultPasswordHash = "AQAAAAIAAYagAAAAEJwrDW1EJCJokjzrCgBRMjaGO3HyGBoCCyg1+2EvfRNQINRgEQYjEx00955bnhvU2g==";


    public static IReadOnlyList<AppUser> All() => new List<AppUser>
    {
        new()
        {
            Id = SuperAdminUserId,
            UserName = SuperAdminUserName,
            NormalizedUserName = SuperAdminUserName.ToUpperInvariant(),
            Email = SuperAdminEmail,
            NormalizedEmail = SuperAdminEmail.ToUpperInvariant(),
            EmailConfirmed = true,
            FirstName = SuperAdminFirstName,
            LastName = SuperAdminLastName,
            SecurityStamp = "USER-SECURITY-SUPERADMIN",
            ConcurrencyStamp = "USER-CONCURRENCY-SUPERADMIN",
            CreatedAt = SeedDates.Anchor,
            PasswordHash = DefaultPasswordHash
        }
    };

    public static IReadOnlyList<IdentityUserRole<Guid>> SuperAdminAssignment() => new List<IdentityUserRole<Guid>>
    {
        new() { UserId = SuperAdminUserId, RoleId = RoleSeed.SuperAdminRoleId }
    };
}

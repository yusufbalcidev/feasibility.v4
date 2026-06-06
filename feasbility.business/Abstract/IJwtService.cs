using System.Security.Claims;
using feasibility.Entity.Entities.Identity;

namespace feasibility.Business.Abstract;

public interface IJwtService
{
    (string token, DateTime expiresAt) GenerateToken(AppUser user, IEnumerable<string> roles);
    IEnumerable<Claim> BuildClaims(AppUser user, IEnumerable<string> roles);
}

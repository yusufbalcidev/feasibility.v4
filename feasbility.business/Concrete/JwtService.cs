using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using feasibility.Business.Abstract;
using feasibility.Entity.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace feasibility.Business.Concrete;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<Claim> BuildClaims(AppUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("FirstName", user.FirstName ?? string.Empty),
            new("LastName", user.LastName ?? string.Empty)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        return claims;
    }

    public (string token, DateTime expiresAt) GenerateToken(AppUser user, IEnumerable<string> roles)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey ayarı bulunamadı.");

        var hours = _configuration.GetValue("JwtSettings:ExpirationInHours", 4);
        var expiresAt = DateTime.UtcNow.AddHours(hours);

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: BuildClaims(user, roles),
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

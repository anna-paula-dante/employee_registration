using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PeopleManager.Domain.Entities;

namespace PeopleManager.Api.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _minutes;

    public JwtTokenService(IConfiguration cfg)
    {
        _key = cfg["Jwt:Key"] ?? cfg["Jwt__Key"] ?? "super-secret-key-change-me";
        _issuer = cfg["Jwt:Issuer"] ?? "people-manager";
        _audience = cfg["Jwt:Audience"] ?? "people-manager-clients";
        _minutes = int.TryParse(cfg["Jwt:Minutes"], out var m) ? m : 120;
    }

    public string Generate(Employee user, out DateTime expiresAt)
    {
        var handler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.UTF8.GetBytes(_key);
        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("document", user.DocumentNumber),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        expiresAt = DateTime.UtcNow.AddMinutes(_minutes);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: creds
        );

        return handler.WriteToken(token);
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleManager.Api.Services;
using PeopleManager.Infrastructure;

namespace PeopleManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;

    public AuthController(AppDbContext db, IJwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (req is null || string.IsNullOrWhiteSpace(req.EmailOrDocument) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { message = "Informe email/documento e senha." });

        var login = req.EmailOrDocument.Trim();

        var user = await _db.Employees.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email == login || e.DocumentNumber == login);

        if (user is null)
            return Unauthorized(new { message = "Credenciais inválidas." });

        var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!ok)
            return Unauthorized(new { message = "Credenciais inválidas." });

        var token = _jwt.Generate(user, out var expiresAt);

        return Ok(new
        {
            access_token = token,
            token_type = "Bearer",
            expires_at = expiresAt,
            user = new { id = user.Id, name = $"{user.FirstName} {user.LastName}", email = user.Email, role = user.Role.ToString() }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var claims = User?.Claims?.ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>();
        return Ok(new { claims });
    }
}

public sealed class LoginRequest
{
    [Required] public string EmailOrDocument { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

using PeopleManager.Infrastructure;
using PeopleManager.Domain.Entities;
using PeopleManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
      .Enrich.FromLogContext()
      .WriteTo.Console();
});

string GetEnv(string key, string fallback) =>
    Environment.GetEnvironmentVariable(key) ?? fallback;

var dbHost = GetEnv("DB_HOST", "localhost");
var dbPort = GetEnv("DB_PORT", "5432");
var dbUser = GetEnv("DB_USER", "postgres");
var dbPass = GetEnv("DB_PASSWORD", "postgres");
var dbName = GetEnv("DB_NAME", "people");

var jwtKey = Environment.GetEnvironmentVariable("Jwt:Key")
          ?? Environment.GetEnvironmentVariable("Jwt__Key")
          ?? "super-secret-key-change-me";

string AdminEnv(string k, string fallback)
{
    var v = Environment.GetEnvironmentVariable("ADMIN_" + k)
         ?? Environment.GetEnvironmentVariable("Admin__" + k);
    return string.IsNullOrWhiteSpace(v) ? fallback : v;
}
var adminFirst = AdminEnv("FIRST", "System");
var adminLast  = AdminEnv("LAST", "Administrator");
var adminEmail = AdminEnv("EMAIL", "admin@peoplemanager.com");
var adminDoc   = AdminEnv("DOCUMENT", "00000000000");
var adminBirth = AdminEnv("BIRTHDATE", "1990-01-01");
var adminPass  = AdminEnv("PASSWORD", "Admin@12345");

var conn = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPass};Database={dbName};";

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(conn));

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(o =>
    {
        o.InvalidModelStateResponseFactory = ctx =>
        {
            var errors = ctx.ModelState
                .Where(kv => kv.Value?.Errors.Count > 0)
                .ToDictionary(kv => kv.Key, kv => kv.Value!.Errors.Select(e => e.ErrorMessage));
            return new BadRequestObjectResult(new { message = "Validation failed", errors });
        };
    });

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("LeaderOrAbove", p => p.RequireRole(nameof(RoleLevel.Leader), nameof(RoleLevel.Director)));
    options.AddPolicy("DirectorOnly",  p => p.RequireRole(nameof(RoleLevel.Director)));
});

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// CORS: permitir o frontend (Vite)
var frontendUrls = (Environment.GetEnvironmentVariable("FRONTEND_URLS")
	               ?? "http://localhost:5173,http://127.0.0.1:5173")
	              .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins(frontendUrls)
		      .AllowAnyHeader()
		      .AllowAnyMethod();
	});
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "People Manager API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "People Manager API v1");
    s.RoutePrefix = "swagger";
});

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

await EnsureDatabaseAndSeedAsync(app.Services, adminFirst, adminLast, adminEmail, adminDoc, adminBirth, adminPass);

app.Run();

static async Task EnsureDatabaseAndSeedAsync(IServiceProvider services, string first, string last, string email, string document, string birthDate, string password)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Garantir que o banco e tabelas existam
    await db.Database.EnsureCreatedAsync();

    if (!DateTime.TryParse(birthDate, out var birth))
        birth = new DateTime(1990,1,1,0,0,0,DateTimeKind.Utc);
    else
        birth = DateTime.SpecifyKind(birth, DateTimeKind.Utc);

    var exists = await db.Employees.AnyAsync(e => e.Email == email || e.DocumentNumber == document);
    if (!exists)
    {
        var hashed = BCrypt.Net.BCrypt.HashPassword(password);
        var admin = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = first,
            LastName = last,
            Email = email,
            DocumentNumber = document,
            BirthDate = birth,
            PasswordHash = hashed,
            Role = RoleLevel.Director,
            ManagerId = null
        };
        db.Employees.Add(admin);
        await db.SaveChangesAsync();
        Console.WriteLine($"[seed] admin created -> {email}");
    }
    else
    {
        Console.WriteLine("[seed] admin exists -> skipping");
    }
}

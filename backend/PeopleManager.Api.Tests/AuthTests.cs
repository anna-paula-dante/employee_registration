using System.Net.Http.Json;
using PeopleManager.Domain.Entities;
using PeopleManager.Infrastructure;

namespace PeopleManager.Api.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public AuthTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Login_Should_Return_Token_For_Valid_Admin()
    {
        var client = _factory.CreateClient();
        // seed admin into in-memory db
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Employees.Add(new Employee{
                FirstName="System", LastName="Admin", Email="admin@peoplemanager.com",
                DocumentNumber="000", BirthDate=DateTime.UtcNow.AddYears(-30),
                PasswordHash=BCrypt.Net.BCrypt.HashPassword("Admin@12345"),
                Role=RoleLevel.Director
            });
            await db.SaveChangesAsync();
        }

        var res = await client.PostAsJsonAsync("/api/v1/auth/login", new { emailOrDocument="admin@peoplemanager.com", password="Admin@12345" });
        res.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var payload = await res.Content.ReadFromJsonAsync<Dictionary<string,object>>();
        payload.Should().ContainKey("access_token");
    }
}

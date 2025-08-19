using System.Net.Http.Headers;
using System.Net.Http.Json;
using PeopleManager.Application.DTOs;
using PeopleManager.Domain.Entities;
using PeopleManager.Infrastructure;

namespace PeopleManager.Api.Tests;

public class AgeValidationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public AgeValidationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Underage_Create_Should_Be_BadRequest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Employees.Add(new Employee{
                FirstName="Sys", LastName="Admin", Email="admin@x.com",
                DocumentNumber="999", BirthDate=DateTime.UtcNow.AddYears(-35),
                PasswordHash=BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role=RoleLevel.Director
            });
            await db.SaveChangesAsync();
        }

        var client = _factory.CreateClient();
        var resLogin = await client.PostAsJsonAsync("/api/v1/auth/login", new { emailOrDocument="admin@x.com", password="Admin@123" });
        var token = (await resLogin.Content.ReadFromJsonAsync<Dictionary<string,object>>())?["access_token"]?.ToString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var body = new CreateEmployeeRequest{
            FirstName="Kid", LastName="User", Email="kid@x.com", DocumentNumber="555",
            BirthDate=DateTime.UtcNow.AddYears(-16), Password="Kid@123", Role=RoleLevel.Employee,
            Phones=new List<string>{"1","2"}
        };
        var res = await client.PostAsJsonAsync("/api/v1/employees", body);
        res.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}

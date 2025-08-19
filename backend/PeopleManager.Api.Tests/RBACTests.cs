using System.Net.Http.Headers;
using System.Net.Http.Json;
using PeopleManager.Domain.Entities;
using PeopleManager.Infrastructure;
using PeopleManager.Application.DTOs;

namespace PeopleManager.Api.Tests;

public class RBACTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public RBACTests(CustomWebApplicationFactory factory) => _factory = factory;

    private async Task<string> LoginAsAsync(string email, string password)
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/v1/auth/login", new { emailOrDocument = email, password });
        var dict = await res.Content.ReadFromJsonAsync<Dictionary<string,object>>() ?? new();
        return dict.TryGetValue("access_token", out var t) ? t?.ToString() ?? "" : "";
    }

    [Fact]
    public async Task Leader_Cannot_Create_Director()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Employees.Add(new Employee{
                FirstName="Dir", LastName="Boss", Email="director@x.com",
                DocumentNumber="111", BirthDate=DateTime.UtcNow.AddYears(-40),
                PasswordHash=BCrypt.Net.BCrypt.HashPassword("Pass@123"), Role=RoleLevel.Director
            });
            db.Employees.Add(new Employee{
                FirstName="Lead", LastName="User", Email="leader@x.com",
                DocumentNumber="222", BirthDate=DateTime.UtcNow.AddYears(-30),
                PasswordHash=BCrypt.Net.BCrypt.HashPassword("Lead@123"), Role=RoleLevel.Leader
            });
            await db.SaveChangesAsync();
        }
        var token = await LoginAsAsync("leader@x.com", "Lead@123");
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var body = new CreateEmployeeRequest{
            FirstName="New", LastName="Director", Email="newdir@x.com", DocumentNumber="333",
            BirthDate=DateTime.UtcNow.AddYears(-25), Password="Dir@123", Role=RoleLevel.Director,
            Phones=new List<string>{"+111","+222"}
        };
        var res = await client.PostAsJsonAsync("/api/v1/employees", body);
        res.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Leader_Can_Create_Employee()
    {
        var token = await LoginAsAsync("leader@x.com", "Lead@123");
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var body = new CreateEmployeeRequest{
            FirstName="John", LastName="Doe", Email="jdoe@x.com", DocumentNumber="444",
            BirthDate=DateTime.UtcNow.AddYears(-20), Password="Emp@123", Role=RoleLevel.Employee,
            Phones=new List<string>{"111","222"}
        };
        var res = await client.PostAsJsonAsync("/api/v1/employees", body);
        res.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }
}

using PeopleManager.Domain.Entities;

namespace PeopleManager.Api.Services;

public interface IJwtTokenService
{
    string Generate(Employee user, out DateTime expiresAt);
}

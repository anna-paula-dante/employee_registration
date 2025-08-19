using System.ComponentModel.DataAnnotations;

namespace PeopleManager.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public string DocumentNumber { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public RoleLevel Role { get; set; } = RoleLevel.Employee;

    public Guid? ManagerId { get; set; }
    public Employee? Manager { get; set; }

    public List<EmployeePhone> Phones { get; set; } = new();
}

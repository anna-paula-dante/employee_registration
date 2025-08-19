using System.ComponentModel.DataAnnotations;
using PeopleManager.Domain.Entities;

namespace PeopleManager.Application.DTOs;

public class CreateEmployeeRequest
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName  { get; set; } = string.Empty;
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string DocumentNumber { get; set; } = string.Empty;
    [Required] public DateTime BirthDate { get; set; }
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public RoleLevel Role { get; set; } = RoleLevel.Employee;
    public Guid? ManagerId { get; set; }
    [MinLength(2)] public List<string> Phones { get; set; } = new();
}

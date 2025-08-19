using System.ComponentModel.DataAnnotations;

namespace PeopleManager.Domain.Entities;

public class EmployeePhone
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(30)]
    public string Number { get; set; } = string.Empty;

    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}

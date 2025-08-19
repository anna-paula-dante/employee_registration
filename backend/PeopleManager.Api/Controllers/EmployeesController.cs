using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleManager.Application.DTOs;
using PeopleManager.Domain.Entities;
using PeopleManager.Infrastructure;
using System.Security.Claims;

namespace PeopleManager.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;
    public EmployeesController(AppDbContext db) { _db = db; }

    private static RoleLevel GetCurrentRole(ClaimsPrincipal user)
    {
        var r = user.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse<RoleLevel>(r, true, out var rl) ? rl : RoleLevel.Employee;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var q = _db.Employees.AsNoTracking().Include(e => e.Phones).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(e => e.FirstName.Contains(s) || e.LastName.Contains(s) || e.Email.Contains(s) || e.DocumentNumber.Contains(s));
        }
        var total = await q.CountAsync();
        var items = await q.OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
                           .Skip((page-1)*pageSize).Take(pageSize)
                           .Select(e => new {
                               e.Id, e.FirstName, e.LastName, e.Email, e.DocumentNumber,
                               e.BirthDate, Role = e.Role.ToString(),
                               Phones = e.Phones.Select(p => p.Number).ToList()
                           }).ToListAsync();
        return Ok(new { items, total, page, pageSize });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var e = await _db.Employees.Include(x => x.Phones).FirstOrDefaultAsync(x => x.Id == id);
        if (e is null) return NotFound();
        return Ok(new {
            e.Id, e.FirstName, e.LastName, e.Email, e.DocumentNumber,
            e.BirthDate, Role = e.Role.ToString(), e.ManagerId,
            Phones = e.Phones.Select(p => p.Number).ToList()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest req)
    {
        // validations
        if (req.BirthDate > DateTime.UtcNow.AddYears(-18))
            return BadRequest(new { message = "Funcionário deve ser maior de 18 anos." });
        if (req.Phones is null || req.Phones.Count < 2)
            return BadRequest(new { message = "Informe ao menos dois telefones." });

        if (await _db.Employees.AnyAsync(e => e.Email == req.Email))
            return Conflict(new { message = "E-mail já cadastrado." });
        if (await _db.Employees.AnyAsync(e => e.DocumentNumber == req.DocumentNumber))
            return Conflict(new { message = "Documento já cadastrado." });

        if (req.ManagerId.HasValue && !await _db.Employees.AnyAsync(e => e.Id == req.ManagerId.Value))
            return BadRequest(new { message = "Gestor informado não existe." });

        var currentRole = GetCurrentRole(User);
        if (req.Role > currentRole)
            return Forbid();

        var emp = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            DocumentNumber = req.DocumentNumber,
            BirthDate = req.BirthDate,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = req.Role,
            ManagerId = req.ManagerId
        };
        emp.Phones = req.Phones.Select(p => new EmployeePhone { Number = p, EmployeeId = emp.Id }).ToList();

        _db.Employees.Add(emp);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = emp.Id }, new { id = emp.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest req)
    {
        var emp = await _db.Employees.Include(e => e.Phones).FirstOrDefaultAsync(e => e.Id == id);
        if (emp is null) return NotFound();

        var currentRole = GetCurrentRole(User);
        if (req.Role > currentRole) return Forbid();

        if (req.BirthDate > DateTime.UtcNow.AddYears(-18))
            return BadRequest(new { message = "Funcionário deve ser maior de 18 anos." });
        if (req.Phones is null || req.Phones.Count < 2)
            return BadRequest(new { message = "Informe ao menos dois telefones." });

        if (await _db.Employees.AnyAsync(e => e.Email == req.Email && e.Id != id))
            return Conflict(new { message = "E-mail já cadastrado." });
        if (await _db.Employees.AnyAsync(e => e.DocumentNumber == req.DocumentNumber && e.Id != id))
            return Conflict(new { message = "Documento já cadastrado." });
        if (req.ManagerId.HasValue && !await _db.Employees.AnyAsync(e => e.Id == req.ManagerId.Value))
            return BadRequest(new { message = "Gestor informado não existe." });

        emp.FirstName = req.FirstName;
        emp.LastName = req.LastName;
        emp.Email = req.Email;
        emp.DocumentNumber = req.DocumentNumber;
        emp.BirthDate = req.BirthDate;
        emp.Role = req.Role;
        emp.ManagerId = req.ManagerId;

        if (!string.IsNullOrWhiteSpace(req.Password))
            emp.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        var newNumbers = req.Phones.ToHashSet();
        var currentPhones = emp.Phones.ToList();

        var toRemove = currentPhones.Where(p => !newNumbers.Contains(p.Number)).ToList();
        _db.EmployeePhones.RemoveRange(toRemove);

        var currentNumbers = currentPhones.Select(p => p.Number).ToHashSet();
        var toAdd = newNumbers.Except(currentNumbers)
            .Select(n => new EmployeePhone { Number = n, EmployeeId = emp.Id })
            .ToList();
        await _db.EmployeePhones.AddRangeAsync(toAdd);

        try
        {
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound(new { message = "Registro inexistente ou alterado por outro processo. Recarregue a página e tente novamente." });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var emp = await _db.Employees.FindAsync(id);
        if (emp is null) return NotFound();
        var currentRole = GetCurrentRole(User);
        if (emp.Role > currentRole) return Forbid();

        _db.Employees.Remove(emp);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

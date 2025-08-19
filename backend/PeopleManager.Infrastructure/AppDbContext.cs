using Microsoft.EntityFrameworkCore;
using PeopleManager.Domain.Entities;

namespace PeopleManager.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeePhone> EmployeePhones => Set<EmployeePhone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.DocumentNumber).IsUnique();
            e.Property(x => x.FirstName).IsRequired();
            e.Property(x => x.LastName).IsRequired();
            e.Property(x => x.Email).IsRequired();
            e.Property(x => x.DocumentNumber).IsRequired();
            e.Property(x => x.PasswordHash).IsRequired();
            e.HasMany(x => x.Phones).WithOne(p => p.Employee!).HasForeignKey(p => p.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Manager).WithMany().HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmployeePhone>(p =>
        {
            p.Property(x => x.Number).IsRequired();
        });
    }
}

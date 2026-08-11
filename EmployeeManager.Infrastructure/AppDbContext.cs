using EmployeeManager.Core.Models;
using EmployeeManager.Infrastructure.DataMocks;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Infrastructure;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }

    //DbSet
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<EmployeeDepartmentAssignment> EmployeeDepartmentAssignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);

            entity.HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DepartmentId);
        });
        modelBuilder.Entity<EmployeeDepartmentAssignment>(entity =>
        {
            entity.HasKey(a => a.AssignmentId);

            entity.Property(a => a.AssignmentId)
                .ValueGeneratedOnAdd();

            entity.Property(a => a.EmployeeId)
                .IsRequired();

            entity.Property(a => a.DepartmentId)
                .IsRequired();

            entity.Property(a => a.AssignmentDate)
                .IsRequired();

            entity.Property(a => a.Status)
                .IsRequired();

            entity.HasOne(a => a.Employee)
                .WithMany(e => e.DepartmentAssignments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Department)
                .WithMany(d => d.EmployeeDepartmentAssignments)
                .HasForeignKey(a => a.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        //Seed the database with mock data. HasData writes these rows into the
        //migration, so they are applied by "dotnet ef database update" (and by
        //Database.Migrate() in the integration tests) - there is no separate seeding step.
        modelBuilder.Entity<Department>().HasData(DepartmentDataMock.GetAllDepartments());
        modelBuilder.Entity<Employee>().HasData(EmployeeDataMock.GetAllEmployees());
    }
}
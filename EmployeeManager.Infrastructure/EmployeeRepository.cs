using EmployeeManager.Application.Repositories;
using EmployeeManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Infrastructure;

public class EmployeeRepository : IEmployeeRepository
{
    protected readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    // AsNoTracking is correct for read-only queries: EF skips building the change
    // tracking graph, which is faster and uses less memory.
    public async Task<List<Employee>> GetAllEmployees(CancellationToken cancellationToken = default)
    {
        var allEmployees = await _context.Employees
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return allEmployees;
    }

    public async Task<Employee?> GetEmployeeById(int id, CancellationToken cancellationToken = default)
    {
        var requestedEmployee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return requestedEmployee;
    }

    public async Task<Employee> CreateEmployee(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        // SaveChangesAsync populates the identity value on the tracked instance,
        // so employee.Id is now the database-generated key.
        return employee;
    }

    public async Task<Employee?> UpdateEmployee(int id, Employee employee, CancellationToken cancellationToken = default)
    {
        // No AsNoTracking here - the entity must be tracked for EF to detect the changes.
        var existing = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (existing is null) return null;

        existing.Name = employee.Name;
        existing.Email = employee.Email;
        existing.DepartmentId = employee.DepartmentId;

        await _context.SaveChangesAsync(cancellationToken);

        return existing;
    }

    public async Task<bool> DeleteEmployeeIfExist(int id, CancellationToken cancellationToken = default)
    {
        // Tracked query: Remove() needs the entity attached to the change tracker.
        var requestedEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (requestedEmployee is null) return false;

        _context.Employees.Remove(requestedEmployee);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DepartmentExists(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .AnyAsync(d => d.Id == departmentId, cancellationToken);
    }
}

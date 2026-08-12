using EmployeeManager.Application.Repositories;
using EmployeeManager.Core.Models;
using EmployeeManager.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    protected readonly AppDbContext _context;

    public AssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeDepartmentAssignment?> GetAssignmentById(int id,CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeDepartmentAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.AssignmentId == id,
                cancellationToken);
    }

    public async Task<bool> EmployeeExists(int employeeId,CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .AnyAsync(
                e => e.Id == employeeId,
                cancellationToken);
    }

    public async Task<bool> DepartmentExists(int departmentId,CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .AsNoTracking()
            .AnyAsync(
                d => d.Id == departmentId,
                cancellationToken);
    }

    public async Task<Employee?> GetEmployeeById(int employeeId,CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.Id == employeeId,
                cancellationToken);
    }

    public async Task<EmployeeDepartmentAssignment> CreateAssignment(EmployeeDepartmentAssignment assignment,CancellationToken cancellationToken = default)
    {
        _context.EmployeeDepartmentAssignments.Add(assignment);

        await _context.SaveChangesAsync(cancellationToken);

        return assignment;
    }

    public async Task<EmployeeDepartmentAssignment?> UpdateAssignment(
        int id,
        DateTime assignmentDate,
        AssignmentStatus status,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.EmployeeDepartmentAssignments
            .FirstOrDefaultAsync(
                a => a.AssignmentId == id,
                cancellationToken);

        if (existing is null)
        {
            return null;
        }

        existing.AssignmentDate = assignmentDate;
        existing.Status = status;

        await _context.SaveChangesAsync(cancellationToken);

        return existing;
    }

    public async Task<bool> DeleteAssignmentIfExist(int id,CancellationToken cancellationToken = default)
    {
        var assignment = await _context.EmployeeDepartmentAssignments
            .FirstOrDefaultAsync(a => a.AssignmentId == id,cancellationToken);

        if (assignment is null)
        {
            return false;
        }

        _context.EmployeeDepartmentAssignments.Remove(assignment);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> HasActiveAssignment(int employeeId,int excludeAssignmentId,CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeDepartmentAssignments
            .AsNoTracking()
            .AnyAsync(
                a => a.EmployeeId == employeeId
                  && a.AssignmentId != excludeAssignmentId
                  && a.Status == AssignmentStatus.Active,
                cancellationToken);
    }
}

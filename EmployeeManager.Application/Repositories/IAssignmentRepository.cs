using EmployeeManager.Core.Models;

namespace EmployeeManager.Application.Repositories;

public interface IAssignmentRepository
{
    Task<EmployeeDepartmentAssignment?> GetAssignmentById(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> EmployeeExists(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<bool> DepartmentExists(
        int departmentId,
        CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeById(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<EmployeeDepartmentAssignment> CreateAssignment(
        EmployeeDepartmentAssignment assignment,
        CancellationToken cancellationToken = default);

    Task<EmployeeDepartmentAssignment?> UpdateAssignment(
        int id,
        DateTime assignmentDate,
        AssignmentStatus status,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAssignmentIfExist(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveAssignment(
        int employeeId,
        int excludeAssignmentId,
        CancellationToken cancellationToken = default);
}
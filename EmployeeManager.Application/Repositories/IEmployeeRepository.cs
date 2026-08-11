using EmployeeManager.Core.Models;

namespace EmployeeManager.Application.Repositories;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllEmployees(CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeById(int id, CancellationToken cancellationToken = default);

    Task<Employee> CreateEmployee(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies the supplied values to the employee with the given id.
    /// Returns null when no employee with that id exists.
    /// </summary>
    Task<Employee?> UpdateEmployee(int id, Employee employee, CancellationToken cancellationToken = default);

    Task<bool> DeleteEmployeeIfExist(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Used to validate a foreign key before insert or update, so that an invalid
    /// DepartmentId produces a clean 400 response instead of a database exception.
    /// </summary>
    Task<bool> DepartmentExists(int departmentId, CancellationToken cancellationToken = default);
}

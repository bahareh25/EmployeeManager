namespace EmployeeManager.Core.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<EmployeeDepartmentAssignment> EmployeeDepartmentAssignments { get; set; }
        = new List<EmployeeDepartmentAssignment>();
}
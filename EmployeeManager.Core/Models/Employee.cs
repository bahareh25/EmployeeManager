namespace EmployeeManager.Core.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    //Navigation Property
    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;
    public ICollection<EmployeeDepartmentAssignment> DepartmentAssignments { get; set; }
        = new List<EmployeeDepartmentAssignment>();
}
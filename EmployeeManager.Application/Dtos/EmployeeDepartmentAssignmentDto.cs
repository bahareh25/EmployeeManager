using EmployeeManager.Core.Models;

namespace EmployeeManager.Application.Dtos;

public class EmployeeDepartmentAssignmentDto
{
    public int AssignmentId { get; set; }

    public int EmployeeId { get; set; }

    public int DepartmentId { get; set; }

    public DateTime AssignmentDate { get; set; }

    public AssignmentStatus Status { get; set; }
}

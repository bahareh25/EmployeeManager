using EmployeeManager.Core.Models;

namespace EmployeeManager.Application.Dtos;

public class CreateEmployeeDepartmentAssignmentDto
{
    public int EmployeeId { get; set; }

    public int DepartmentId { get; set; }

    public DateTime AssignmentDate { get; set; }
}
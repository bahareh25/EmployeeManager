using EmployeeManager.Core.Models;

namespace EmployeeManager.Application.Dtos;

public record EmployeeDepartmentAssignmentDto
(int AssignmentId, int EmployeeId, int DepartmentId, DateTime AssignmentDate, AssignmentStatus Status);


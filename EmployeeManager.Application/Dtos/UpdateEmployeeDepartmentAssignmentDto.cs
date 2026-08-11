using System.ComponentModel.DataAnnotations;
using EmployeeManager.Core.Models;

namespace EmployeeManager.Application.Dtos;

public class UpdateEmployeeDepartmentAssignmentDto
{
    public DateTime AssignmentDate { get; set; }

    [Required]
    public AssignmentStatus Status { get; set; }
}
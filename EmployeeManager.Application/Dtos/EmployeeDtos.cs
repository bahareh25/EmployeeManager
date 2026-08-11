using System.ComponentModel.DataAnnotations;

namespace EmployeeManager.Application.Dtos;

/// <summary>
/// What the API returns for an employee.
/// </summary>
/// <remarks>
/// Controllers return DTOs, never EF entities. Two reasons:
/// 1. Entities carry navigation properties that form cycles when serialized.
/// 2. The API contract stays stable even if the database schema changes.
/// Follow this pattern for the assignment entity you add.
/// </remarks>
public record EmployeeResponse(int Id, string Name, string Email, int DepartmentId);

/// <summary>
/// Request body for POST /api/employees.
/// </summary>
/// <remarks>
/// The [ApiController] attribute on the controller automatically returns
/// 400 Bad Request with a ValidationProblemDetails body when any of these
/// data annotations fail, so no manual ModelState.IsValid check is needed.
/// </remarks>
public class CreateEmployeeRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be a positive integer.")]
    public int DepartmentId { get; set; }
}

/// <summary>
/// Request body for PUT /api/employees/{id}. The id comes from the route, not the body.
/// </summary>
public class UpdateEmployeeRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be a positive integer.")]
    public int DepartmentId { get; set; }
}

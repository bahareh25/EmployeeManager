using System.Text.Json;
using EmployeeManager.Core.Models;

namespace EmployeeManager.Infrastructure.DataMocks;

/// <summary>
/// Seed data applied by EF Core migrations through ModelBuilder.HasData.
/// </summary>
/// <remarks>
/// These methods are synchronous on purpose. They read from an in-memory constant,
/// so there is nothing to await - and OnModelCreating cannot await anything anyway.
/// Returning a Task only to call .Result on it blocks a thread pool thread for no
/// benefit, which is the sync-over-async antipattern.
/// </remarks>
public static class DepartmentDataMock
{
    private const string DepartmentData = """
    [
        { "Id": 1,  "Name": "Human Resources" },
        { "Id": 2,  "Name": "Finance" },
        { "Id": 3,  "Name": "Engineering" },
        { "Id": 4,  "Name": "Marketing" },
        { "Id": 5,  "Name": "Sales" },
        { "Id": 6,  "Name": "Customer Support" },
        { "Id": 7,  "Name": "IT" },
        { "Id": 8,  "Name": "Research and Development" },
        { "Id": 9,  "Name": "Operations" },
        { "Id": 10, "Name": "Legal" }
    ]
    """;

    public static List<Department> GetAllDepartments() =>
        JsonSerializer.Deserialize<List<Department>>(DepartmentData) ?? [];

    public static Department? GetDepartmentById(int id) =>
        GetAllDepartments().FirstOrDefault(d => d.Id == id);
}

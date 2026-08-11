using System.Net;
using System.Net.Http.Json;
using EmployeeManager.Application.Dtos;
using EmployeeManagerApi.IntegrationTests.Urls;
using FluentAssertions;

namespace EmployeeManagerApi.IntegrationTests;

/// <summary>
/// Worked examples of integration tests against the real API and a real database.
/// </summary>
/// <remarks>
/// Use this file as the template for the tests you write in Goal 4. Note the shape:
/// the class joins the "Integration" collection, takes the shared fixture through the
/// constructor, and never constructs a web application factory itself.
/// </remarks>
[Collection(IntegrationTestCollection.Name)]
public class EmployeeControllerTests
{
    private readonly HttpClient _client;

    public EmployeeControllerTests(ApiTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GetEmployees_WhenEmployeesExist_ShouldReturnOkWithEmployees()
    {
        //Act
        var response = await _client.GetAsync(ApiRoutes.Employees.Base);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var employees = await response.Content.ReadFromJsonAsync<List<EmployeeResponse>>();
        employees.Should().NotBeNull();
        employees!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetEmployeeById_WhenEmployeeExists_ShouldReturnOkWithThatEmployee()
    {
        //Arrange - employee 1 comes from the seed data applied by the migration.
        const int existingEmployeeId = 1;

        //Act
        var response = await _client.GetAsync(ApiRoutes.Employees.ById(existingEmployeeId));

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var employee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
        employee.Should().NotBeNull();
        employee!.Id.Should().Be(existingEmployeeId);
    }

    [Fact]
    public async Task GetEmployeeById_WhenEmployeeDoesNotExist_ShouldReturnNotFound()
    {
        //Arrange
        const int missingEmployeeId = 999_999;

        //Act
        var response = await _client.GetAsync(ApiRoutes.Employees.ById(missingEmployeeId));

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateEmployee_WithValidPayload_ShouldReturnCreatedAndBeRetrievable()
    {
        //Arrange
        var request = new CreateEmployeeRequest
        {
            Name = "Test Person",
            Email = "test.person@acme-corp.com",
            DepartmentId = 7
        };

        //Act
        var response = await _client.PostAsJsonAsync(ApiRoutes.Employees.Base, request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var created = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);
        created.Name.Should().Be(request.Name);

        //Assert the resource really was persisted, not just echoed back.
        var followUp = await _client.GetAsync(ApiRoutes.Employees.ById(created.Id));
        followUp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateEmployee_WithUnknownDepartment_ShouldReturnBadRequest()
    {
        //Arrange
        var request = new CreateEmployeeRequest
        {
            Name = "Test Person",
            Email = "test.person@acme-corp.com",
            DepartmentId = 999_999
        };

        //Act
        var response = await _client.PostAsJsonAsync(ApiRoutes.Employees.Base, request);

        //Assert - a foreign key that does not resolve is a client error, not a 500.
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateEmployee_WhenIdIsUnknownAndBodyIsInvalid_ShouldReturnNotFound()
    {
        //Arrange - two problems in one request: the id does not exist and the department
        //does not exist either. Precedence says the 404 wins over the 400.
        var request = new UpdateEmployeeRequest
        {
            Name = "Test Person",
            Email = "test.person@acme-corp.com",
            DepartmentId = 999_999
        };

        //Act
        var response = await _client.PutAsJsonAsync(ApiRoutes.Employees.ById(999_999), request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteEmployee_WhenEmployeeDoesNotExist_ShouldReturnNotFound()
    {
        //Arrange
        const int missingEmployeeId = 999_999;

        //Act
        var response = await _client.DeleteAsync(ApiRoutes.Employees.ById(missingEmployeeId));

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    //TODO (Goal 4): add EmployeeDepartmentAssignmentControllerTests in a new file,
    //following this same pattern, covering business rules BR-01 through BR-05.
}

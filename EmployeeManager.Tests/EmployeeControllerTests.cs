using EmployeeManager.API.Controllers;
using EmployeeManager.Application.Dtos;
using EmployeeManager.Application.Repositories;
using EmployeeManager.Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManager.Tests;

/// <summary>
/// Unit tests for the controller in isolation: the repository is mocked, so nothing
/// here touches a database. Compare with EmployeeManagerApi.IntegrationTests, which
/// exercises the same endpoints end to end.
/// </summary>
public class EmployeeControllerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly EmployeeController _employeeController;
    private readonly Mock<ILogger<EmployeeController>> _loggerMock;

    public EmployeeControllerTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _loggerMock = new Mock<ILogger<EmployeeController>>();
        _employeeController = new EmployeeController(_loggerMock.Object, _employeeRepositoryMock.Object);
    }

    private static Employee SampleEmployee(int id = 1, int departmentId = 4) =>
        new() { Id = id, Name = "Damian Martin", Email = "damian@ymail.com", DepartmentId = departmentId };

    [Fact]
    public async Task GetEmployees_WhenEmployeesExist_ShouldReturnOkWithEmployees()
    {
        //Arrange
        var cancellationToken = CancellationToken.None;
        var employees = new List<Employee> { SampleEmployee(1), SampleEmployee(2) };

        _employeeRepositoryMock.Setup(repo => repo.GetAllEmployees(cancellationToken))
            .ReturnsAsync(employees);

        //Act
        var result = await _employeeController.GetEmployees(cancellationToken);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<List<EmployeeResponse>>(okResult.Value);

        body.Should().HaveCount(2);

        //check whether the repository was called Only once
        _employeeRepositoryMock.Verify(repo => repo.GetAllEmployees(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetEmployees_WhenNoEmployeesExist_ShouldReturnOkWithEmptyList()
    {
        //Arrange
        var cancellationToken = CancellationToken.None;

        _employeeRepositoryMock.Setup(repo => repo.GetAllEmployees(cancellationToken))
            .ReturnsAsync(new List<Employee>());

        //Act
        var result = await _employeeController.GetEmployees(cancellationToken);

        //Assert - an empty collection is 200 with [], not 404. 404 would mean the
        //endpoint itself does not exist.
        var okResult = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<List<EmployeeResponse>>(okResult.Value);

        body.Should().BeEmpty();

        _employeeRepositoryMock.Verify(repo => repo.GetAllEmployees(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeById_WhenEmployeeExists_ShouldReturnOkWithEmployee()
    {
        //Arrange
        const int id = 10;
        var cancellationToken = CancellationToken.None;

        _employeeRepositoryMock.Setup(repo => repo.GetEmployeeById(id, cancellationToken))
            .ReturnsAsync(SampleEmployee(id));

        //Act
        var result = await _employeeController.GetEmployeeById(id, cancellationToken);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<EmployeeResponse>(okResult.Value);

        body.Id.Should().Be(id);

        _employeeRepositoryMock.Verify(repo => repo.GetEmployeeById(id, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeById_WhenEmployeeDoesNotExist_ShouldReturnNotFound()
    {
        //Arrange
        const int id = 1000000;
        var cancellationToken = CancellationToken.None;

        _employeeRepositoryMock.Setup(repo => repo.GetEmployeeById(id, cancellationToken))
            .ReturnsAsync((Employee?)null);

        //Act
        var result = await _employeeController.GetEmployeeById(id, cancellationToken);

        //Assert
        Assert.IsType<NotFoundResult>(result);

        _employeeRepositoryMock.Verify(repo => repo.GetEmployeeById(id, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateEmployee_WithValidRequest_ShouldReturnCreatedAtAction()
    {
        //Arrange
        var cancellationToken = CancellationToken.None;
        var request = new CreateEmployeeRequest
        {
            Name = "Damian Martin",
            Email = "damian@ymail.com",
            DepartmentId = 4
        };

        _employeeRepositoryMock.Setup(repo => repo.DepartmentExists(request.DepartmentId, cancellationToken))
            .ReturnsAsync(true);

        _employeeRepositoryMock.Setup(repo => repo.CreateEmployee(It.IsAny<Employee>(), cancellationToken))
            .ReturnsAsync(SampleEmployee(42));

        //Act
        var result = await _employeeController.CreateEmployee(request, cancellationToken);

        //Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var body = Assert.IsType<EmployeeResponse>(createdResult.Value);

        body.Id.Should().Be(42);

        _employeeRepositoryMock.Verify(repo => repo.CreateEmployee(It.IsAny<Employee>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateEmployee_WhenDepartmentDoesNotExist_ShouldReturnBadRequest()
    {
        //Arrange
        var cancellationToken = CancellationToken.None;
        var request = new CreateEmployeeRequest
        {
            Name = "Damian Martin",
            Email = "damian@ymail.com",
            DepartmentId = 999
        };

        _employeeRepositoryMock.Setup(repo => repo.DepartmentExists(request.DepartmentId, cancellationToken))
            .ReturnsAsync(false);

        //Act
        var result = await _employeeController.CreateEmployee(request, cancellationToken);

        //Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<ValidationProblemDetails>(badRequest.Value);

        //Nothing should have been written when validation fails.
        _employeeRepositoryMock.Verify(repo => repo.CreateEmployee(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateEmployee_WhenEmployeeDoesNotExist_ShouldReturnNotFound()
    {
        //Arrange
        const int id = 1000000;
        var cancellationToken = CancellationToken.None;
        var request = new UpdateEmployeeRequest
        {
            Name = "Damian Martin",
            Email = "damian@ymail.com",
            DepartmentId = 4
        };

        _employeeRepositoryMock.Setup(repo => repo.GetEmployeeById(id, cancellationToken))
            .ReturnsAsync((Employee?)null);

        _employeeRepositoryMock.Setup(repo => repo.DepartmentExists(request.DepartmentId, cancellationToken))
            .ReturnsAsync(true);

        //Act
        var result = await _employeeController.UpdateEmployee(id, request, cancellationToken);

        //Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateEmployee_WhenEmployeeMissingAndDepartmentInvalid_ShouldReturnNotFoundNotBadRequest()
    {
        //Arrange - the request breaks two things at once: the id does not resolve AND
        //the body carries an invalid department. Precedence says 404 wins.
        const int id = 1000000;
        var cancellationToken = CancellationToken.None;
        var request = new UpdateEmployeeRequest
        {
            Name = "Damian Martin",
            Email = "damian@ymail.com",
            DepartmentId = 999
        };

        _employeeRepositoryMock.Setup(repo => repo.GetEmployeeById(id, cancellationToken))
            .ReturnsAsync((Employee?)null);

        _employeeRepositoryMock.Setup(repo => repo.DepartmentExists(request.DepartmentId, cancellationToken))
            .ReturnsAsync(false);

        //Act
        var result = await _employeeController.UpdateEmployee(id, request, cancellationToken);

        //Assert
        Assert.IsType<NotFoundResult>(result);

        //The body should not even have been examined.
        _employeeRepositoryMock.Verify(
            repo => repo.DepartmentExists(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteEmployeeById_WhenEmployeeExists_ShouldReturnNoContent()
    {
        //Arrange
        const int id = 1;
        var cancellationToken = CancellationToken.None;

        _employeeRepositoryMock.Setup(repo => repo.DeleteEmployeeIfExist(id, cancellationToken))
            .ReturnsAsync(true);

        //Act
        var result = await _employeeController.DeleteEmployeeById(id, cancellationToken);

        //Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteEmployeeById_WhenEmployeeDoesNotExist_ShouldReturnNotFound()
    {
        //Arrange
        const int id = 1000000;
        var cancellationToken = CancellationToken.None;

        _employeeRepositoryMock.Setup(repo => repo.DeleteEmployeeIfExist(id, cancellationToken))
            .ReturnsAsync(false);

        //Act
        var result = await _employeeController.DeleteEmployeeById(id, cancellationToken);

        //Assert
        Assert.IsType<NotFoundResult>(result);
    }
}

using EmployeeManager.API.Controllers;
using EmployeeManager.Application.Dtos;
using EmployeeManager.Application.Repositories;
using EmployeeManager.Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManager.Tests
{
    public class EmployeeDepartmentAssignmentControllerTests
    {
        private readonly Mock<IAssignmentRepository> _assignmentRepositoryMock;
        private readonly Mock<ILogger<EmployeeDepartmentAssignmentController>> _loggerMock;
        private readonly EmployeeDepartmentAssignmentController _controller;

        public EmployeeDepartmentAssignmentControllerTests()
        {
            _assignmentRepositoryMock = new Mock<IAssignmentRepository>();
            _loggerMock = new Mock<ILogger<EmployeeDepartmentAssignmentController>>();

            _controller = new EmployeeDepartmentAssignmentController(
                _loggerMock.Object,
                _assignmentRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateAssignment_WhenDateIsMoreThan31DaysInFuture_ShouldReturnBadRequest()
        {
            //Arrange
            var cancellationToken=CancellationToken.None;
            var request = new CreateEmployeeDepartmentAssignmentDto
            {
                EmployeeId = 1,
                DepartmentId = 2,
                AssignmentDate = DateTime.UtcNow.Date.AddDays(32)
            };

            // Employee exists
            _assignmentRepositoryMock
                .Setup(repo => repo.GetEmployeeById(
                    request.EmployeeId,
                    cancellationToken))
                .ReturnsAsync(new Employee
                {
                    Id = 1,
                    DepartmentId = 3
                });

            // Department exists
            _assignmentRepositoryMock
                .Setup(repo => repo.DepartmentExists(
                    request.DepartmentId,
                    cancellationToken))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateAssignment(
                request,
                cancellationToken);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.IsType<ValidationProblemDetails>(
                badRequest.Value);

            // Nothing should be written to the database
            _assignmentRepositoryMock.Verify(
                repo => repo.CreateAssignment(
                    It.IsAny<EmployeeDepartmentAssignment>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAssignment_WhenDepartmentIsEmployeePermanentDepartment_ShouldReturnBadRequest()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var request = new CreateEmployeeDepartmentAssignmentDto
            {
                EmployeeId = 1,
                DepartmentId = 3,
                AssignmentDate = DateTime.UtcNow.Date
            };

            // Employee exists and their permanent department is 3
            _assignmentRepositoryMock
                .Setup(repo => repo.GetEmployeeById(
                    request.EmployeeId,
                    cancellationToken))
                .ReturnsAsync(new Employee
                {
                    Id = 1,
                    DepartmentId = 3
                });

            // Department also exists
            _assignmentRepositoryMock
                .Setup(repo => repo.DepartmentExists(
                    request.DepartmentId,
                    cancellationToken))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateAssignment(
                request,
                cancellationToken);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.IsType<ValidationProblemDetails>(
                badRequest.Value);

            // BR-05 must prevent the database write
            _assignmentRepositoryMock.Verify(
                repo => repo.CreateAssignment(
                    It.IsAny<EmployeeDepartmentAssignment>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAssignment_WhenRequestIsValid_ShouldCreateAssignmentAsScheduled()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var request = new CreateEmployeeDepartmentAssignmentDto
            {
                EmployeeId = 1,
                DepartmentId = 2,
                AssignmentDate = DateTime.UtcNow.Date
            };

            _assignmentRepositoryMock
                .Setup(repo => repo.GetEmployeeById(
                    request.EmployeeId,
                    cancellationToken))
                .ReturnsAsync(new Employee
                {
                    Id = 1,
                    DepartmentId = 3
                });

            _assignmentRepositoryMock
                .Setup(repo => repo.DepartmentExists(
                    request.DepartmentId,
                    cancellationToken))
                .ReturnsAsync(true);

            var createdAssignment = new EmployeeDepartmentAssignment
            {
                AssignmentId = 10,
                EmployeeId = 1,
                DepartmentId = 2,
                AssignmentDate = request.AssignmentDate,
                Status = AssignmentStatus.Scheduled
            };

            _assignmentRepositoryMock
                .Setup(repo => repo.CreateAssignment(
                    It.IsAny<EmployeeDepartmentAssignment>(),
                    cancellationToken))
                .ReturnsAsync(createdAssignment);

            // Act
            var result = await _controller.CreateAssignment(
                request,
                cancellationToken);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            var response = Assert.IsType<EmployeeDepartmentAssignmentDto>(
                createdResult.Value);

            response.Status.Should().Be(AssignmentStatus.Scheduled);

            // Verify the entity sent to the repository has Scheduled status
            _assignmentRepositoryMock.Verify(
                repo => repo.CreateAssignment(
                    It.Is<EmployeeDepartmentAssignment>(assignment =>
                        assignment.Status == AssignmentStatus.Scheduled &&
                        assignment.EmployeeId == request.EmployeeId &&
                        assignment.DepartmentId == request.DepartmentId &&
                        assignment.AssignmentDate == request.AssignmentDate),
                    cancellationToken),
                Times.Once);
        }
        [Theory]
        [InlineData(AssignmentStatus.Scheduled, AssignmentStatus.Active, true)]
        [InlineData(AssignmentStatus.Scheduled, AssignmentStatus.Cancelled, true)]
        [InlineData(AssignmentStatus.Active, AssignmentStatus.Completed, true)]
        [InlineData(AssignmentStatus.Active, AssignmentStatus.Cancelled, true)]
        [InlineData(AssignmentStatus.Completed, AssignmentStatus.Active, false)]
        [InlineData(AssignmentStatus.Completed, AssignmentStatus.Cancelled, false)]
        [InlineData(AssignmentStatus.Cancelled, AssignmentStatus.Active, false)]
        [InlineData(AssignmentStatus.Cancelled, AssignmentStatus.Completed, false)]
        [InlineData(AssignmentStatus.Scheduled, AssignmentStatus.Scheduled, true)]
        [InlineData(AssignmentStatus.Active, AssignmentStatus.Active, true)]
        [InlineData(AssignmentStatus.Completed, AssignmentStatus.Completed, true)]
        [InlineData(AssignmentStatus.Cancelled, AssignmentStatus.Cancelled, true)]
        public async Task UpdateAssignment_ShouldEnforceStatusTransitionRules(AssignmentStatus currentStatus,AssignmentStatus requestedStatus,
                bool shouldBeValid)
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            const int assignmentId = 10;

            var existingAssignment = new EmployeeDepartmentAssignment
            {
                AssignmentId = assignmentId,
                EmployeeId = 1,
                DepartmentId = 2,
                AssignmentDate = DateTime.UtcNow.Date,
                Status = currentStatus
            };

            var request = new UpdateEmployeeDepartmentAssignmentDto
            {
                AssignmentDate = DateTime.UtcNow.Date,
                Status = requestedStatus
            };

            _assignmentRepositoryMock
                .Setup(repo => repo.GetAssignmentById(
                    assignmentId,
                    cancellationToken))
                .ReturnsAsync(existingAssignment);
            _assignmentRepositoryMock
                .Setup(repo => repo.UpdateAssignment(
                                        assignmentId,
                                         request.AssignmentDate,
                                         request.Status,
                                        cancellationToken))
                                .ReturnsAsync(existingAssignment);

            // Act
            var result = await _controller.UpdateAssignment(
                assignmentId,
                request,
                cancellationToken);

            // Assert
            if (shouldBeValid)
            {
                Assert.IsType<OkObjectResult>(result);

                _assignmentRepositoryMock.Verify(
                    repo => repo.UpdateAssignment(
                        assignmentId,
                        request.AssignmentDate,
                        request.Status,
                        cancellationToken),
                    Times.Once);
            }
            else
            {
                var badRequest = Assert.IsType<BadRequestObjectResult>(result);

                Assert.IsType<ValidationProblemDetails>(
                    badRequest.Value);

                // Invalid transition must not reach the database.
                _assignmentRepositoryMock.Verify(
                    repo => repo.UpdateAssignment(
                        It.IsAny<int>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<AssignmentStatus>(),
                        It.IsAny<CancellationToken>()),
                    Times.Never);
            }
        }
        [Fact]
        public async Task UpdateAssignment_WhenAnotherActiveAssignmentExists_ShouldReturnConflict()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            const int assignmentId = 10;
            const int employeeId = 1;

            var existingAssignment = new EmployeeDepartmentAssignment
            {
                AssignmentId = assignmentId,
                EmployeeId = employeeId,
                DepartmentId = 2,
                AssignmentDate = DateTime.UtcNow.Date,
                Status = AssignmentStatus.Scheduled
            };

            var request = new UpdateEmployeeDepartmentAssignmentDto
            {
                AssignmentDate = DateTime.UtcNow.Date,
                Status = AssignmentStatus.Active
            };

            // The assignment we are updating exists.
            _assignmentRepositoryMock
                .Setup(repo => repo.GetAssignmentById(
                    assignmentId,
                    cancellationToken))
                .ReturnsAsync(existingAssignment);

            // Another Active assignment already exists for this employee.
            _assignmentRepositoryMock
                .Setup(repo => repo.HasActiveAssignment(
                    employeeId,
                    assignmentId,
                    cancellationToken))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAssignment(
                assignmentId,
                request,
                cancellationToken);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);

            // The update must NOT be written to the database.
            _assignmentRepositoryMock.Verify(
                repo => repo.UpdateAssignment(
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<AssignmentStatus>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}

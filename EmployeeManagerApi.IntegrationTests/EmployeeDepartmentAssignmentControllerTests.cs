using EmployeeManager.Application.Dtos;
using EmployeeManager.Core.Models;
using EmployeeManagerApi.IntegrationTests.Urls;
using FluentAssertions;
using System.Data;
using System.Net;
using System.Net.Http.Json;
using static EmployeeManagerApi.IntegrationTests.Urls.ApiRoutes;

namespace EmployeeManagerApi.IntegrationTests
{
    [Collection(IntegrationTestCollection.Name)]
    public class EmployeeDepartmentAssignmentControllerTests
    {
        private readonly HttpClient _client;

        public EmployeeDepartmentAssignmentControllerTests(ApiTestFixture fixture)
        {
            _client = fixture.Client;
        }
        //BR02:AssignmentDate must not be more than 31 days in the future.
        [Fact]
        public async Task CreateAssignment_WhenDateIsMoreThan31DaysInFuture_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateEmployeeDepartmentAssignmentDto
            {
                EmployeeId = 1,
                DepartmentId = 2,
                AssignmentDate = DateTime.UtcNow.Date.AddDays(32)
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                ApiRoutes.Assignments.Base,
                request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        //BR-05: Employee cannot be assigned to their permanent department
        [Fact]
        public async Task CreateAssignment_WhenDepartmentIsEmployeePermanentDepartment_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateEmployeeDepartmentAssignmentDto
            {
                EmployeeId = 1,
                DepartmentId = 1, // Employee 1's permanent department
                AssignmentDate = DateTime.UtcNow.Date
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                ApiRoutes.Assignments.Base,
                request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        //BR-03: New assignment must be Scheduled
        [Fact]
        public async Task CreateAssignment_WithValidRequest_ShouldReturnCreatedWithScheduledStatus()
        {
            // Arrange
            var request = new CreateEmployeeDepartmentAssignmentDto
            {
                EmployeeId = 1,
                DepartmentId = 2,
                AssignmentDate = DateTime.UtcNow.Date
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                ApiRoutes.Assignments.Base,
                request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var created = await response.Content.ReadFromJsonAsync<EmployeeDepartmentAssignmentDto>(
                ApiTestFixture.JsonOptions);

            created.Should().NotBeNull();
            created!.AssignmentId.Should().BeGreaterThan(0);

            created.EmployeeId.Should().Be(request.EmployeeId);
            created.DepartmentId.Should().Be(request.DepartmentId);

            created.Status.Should().Be(AssignmentStatus.Scheduled);
        }

        //BR04-Invalid status transition
        [Fact]
        public async Task UpdateAssignment_WhenCompletedAssignmentIsChangedToActive_ShouldReturnBadRequest()
        {
            // Arrange
            var createRequest = new CreateEmployeeDepartmentAssignmentDto
            {
                EmployeeId = 1,
                DepartmentId = 2,
                AssignmentDate = DateTime.UtcNow.Date
            };

            // Create a real assignment through the API.
            var createResponse = await _client.PostAsJsonAsync(
                ApiRoutes.Assignments.Base,
                createRequest);

            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await createResponse.Content.ReadFromJsonAsync<EmployeeDepartmentAssignmentDto>(
                ApiTestFixture.JsonOptions);

            created.Should().NotBeNull();

            // Step 2 — Change Scheduled → Active.
            var activateRequest = new UpdateEmployeeDepartmentAssignmentDto
            {
                AssignmentDate = createRequest.AssignmentDate,
                Status = AssignmentStatus.Active
            };

            var activateResponse = await _client.PutAsJsonAsync(
                ApiRoutes.Assignments.ById(created!.AssignmentId),
                activateRequest);

            activateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Step 3 — Change Active → Completed.
            var completeRequest = new UpdateEmployeeDepartmentAssignmentDto
            {
                AssignmentDate = createRequest.AssignmentDate,
                Status = AssignmentStatus.Completed
            };

            var completeResponse = await _client.PutAsJsonAsync(
                ApiRoutes.Assignments.ById(created.AssignmentId),
                completeRequest);

            completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Step 4 — Try the invalid transition Completed → Active.
            var invalidRequest = new UpdateEmployeeDepartmentAssignmentDto
            {
                AssignmentDate = createRequest.AssignmentDate,
                Status = AssignmentStatus.Active
            };

            var invalidResponse = await _client.PutAsJsonAsync(
                ApiRoutes.Assignments.ById(created.AssignmentId),
                invalidRequest);

            // Assert — BR-04 violation.
            invalidResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        //BR-01- An employee cannot have two Active assignments.
        
                [Fact]
        public async Task UpdateAssignment_WhenEmployeeAlreadyHasActiveAssignment_ShouldReturnConflict()
        {
            // Arrange
            // Create the first assignment.
            var firstCreateRequest = new CreateEmployeeDepartmentAssignmentDto
            {
                EmployeeId = 1,
                DepartmentId = 2,
                AssignmentDate = DateTime.UtcNow.Date
            };

            var firstCreateResponse = await _client.PostAsJsonAsync(
                ApiRoutes.Assignments.Base,
                firstCreateRequest);

            firstCreateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var firstAssignment =
                await firstCreateResponse.Content.ReadFromJsonAsync<EmployeeDepartmentAssignmentDto>(
                    ApiTestFixture.JsonOptions);

            firstAssignment.Should().NotBeNull();

            // Activate the first assignment.
            var activateFirstRequest = new UpdateEmployeeDepartmentAssignmentDto
            {
                AssignmentDate = firstCreateRequest.AssignmentDate,
                Status = AssignmentStatus.Active
            };

            var activateFirstResponse = await _client.PutAsJsonAsync(
                ApiRoutes.Assignments.ById(firstAssignment!.AssignmentId),
                activateFirstRequest);

            activateFirstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Create a second assignment for the SAME employee,
            // but use a different department.
            var secondCreateRequest = new CreateEmployeeDepartmentAssignmentDto
            {
                EmployeeId = 1,
                DepartmentId = 3,
                AssignmentDate = DateTime.UtcNow.Date
            };

            var secondCreateResponse = await _client.PostAsJsonAsync(
                ApiRoutes.Assignments.Base,
                secondCreateRequest);

            secondCreateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var secondAssignment =
                await secondCreateResponse.Content.ReadFromJsonAsync<EmployeeDepartmentAssignmentDto>(
                    ApiTestFixture.JsonOptions);

            secondAssignment.Should().NotBeNull();

            // Act
            // Try to activate the second assignment.
            var activateSecondRequest = new UpdateEmployeeDepartmentAssignmentDto
            {
                AssignmentDate = secondCreateRequest.AssignmentDate,
                Status = AssignmentStatus.Active
            };

            var activateSecondResponse = await _client.PutAsJsonAsync(
                ApiRoutes.Assignments.ById(secondAssignment!.AssignmentId),
                activateSecondRequest);

            // Assert
            // BR-01 requires 409 Conflict.
            activateSecondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}

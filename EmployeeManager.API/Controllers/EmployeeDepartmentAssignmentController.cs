using EmployeeManager.Application.Dtos;
using EmployeeManager.Application.Repositories;
using EmployeeManager.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.Controllers
{
    [Route("api/assignment")]
    [ApiController]
    public class EmployeeDepartmentAssignmentController : ControllerBase
    {
        private readonly ILogger<EmployeeDepartmentAssignmentController> _logger;
        private readonly IAssignmentRepository _assignmentRepository;

        public EmployeeDepartmentAssignmentController(
            ILogger<EmployeeDepartmentAssignmentController> logger,
            IAssignmentRepository assignmentRepository)
        {
            _logger = logger;
            _assignmentRepository = assignmentRepository;
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EmployeeDepartmentAssignmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAssignmentById(int id,CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching assignment with id {id}",id);

            var assignment = await _assignmentRepository.GetAssignmentById(id,cancellationToken);

            if (assignment is null)
            {
                return NotFound();
            }

            return Ok(ToResponse(assignment));
        }
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeDepartmentAssignmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAssignment([FromBody] CreateEmployeeDepartmentAssignmentDto request,CancellationToken cancellationToken)
        {
            // Step 1: Employee must exist
            var employee = await _assignmentRepository.GetEmployeeById(
                request.EmployeeId,
                cancellationToken);

            if (employee is null)
            {
                ModelState.AddModelError(nameof(request.EmployeeId),$"Employee {request.EmployeeId} does not exist.");

                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Step 2: Department must exist
            if (!await _assignmentRepository.DepartmentExists(request.DepartmentId,cancellationToken))
            {
                ModelState.AddModelError(nameof(request.DepartmentId),$"Department {request.DepartmentId} does not exist.");

                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Step 3: BR-02
            // AssignmentDate.Date must not be more than 31 days
            // in the future according to UTC date.
            var maxAllowedDate = DateTime.UtcNow.Date.AddDays(31);

            if (request.AssignmentDate.Date > maxAllowedDate)
            {
                ModelState.AddModelError(nameof(request.AssignmentDate),"Assignment date cannot be more than 31 days in the future.");

                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Step 4: BR-05
            // Employee cannot be temporarily assigned to
            // their own permanent department.
            if (employee.DepartmentId == request.DepartmentId)
            {
                ModelState.AddModelError(nameof(request.DepartmentId),"Employee cannot be temporarily assigned to their permanent department.");

                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Step 5: BR-03
            // Every newly created assignment MUST start as Scheduled.
            var assignment = new EmployeeDepartmentAssignment
            {
                EmployeeId = request.EmployeeId,
                DepartmentId = request.DepartmentId,
                AssignmentDate = request.AssignmentDate,
                Status = AssignmentStatus.Scheduled
            };

            var created = await _assignmentRepository.CreateAssignment(
                assignment,
                cancellationToken);

            _logger.LogInformation(
                "Created employee department assignment with id {id}",
                created.AssignmentId);

            // Step 6: 201 Created + Location header
            return CreatedAtAction(
                nameof(GetAssignmentById),
                new { id = created.AssignmentId },
                ToResponse(created));
        }
       
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(EmployeeDepartmentAssignmentDto),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateAssignment(int id,[FromBody] UpdateEmployeeDepartmentAssignmentDto request,CancellationToken cancellationToken)
        {
            // Step 1: The assignment must exist.
            // This check MUST happen before business-rule validation.
            var existing = await _assignmentRepository.GetAssignmentById( id,cancellationToken);

            if (existing is null)
            {
                return NotFound();
            }

            // Step 2: BR-02
            // Re-check the date only because AssignmentDate is being updated.
            var maxAllowedDate = DateTime.UtcNow.Date.AddDays(31);

            if (request.AssignmentDate.Date > maxAllowedDate)
            {
                ModelState.AddModelError(
                    nameof(request.AssignmentDate),
                    "Assignment date cannot be more than 31 days in the future.");

                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Step 3: BR-04
            // Validate the status transition before checking BR-01.
            if (!IsValidStatusTransition(existing.Status, request.Status))
            {
                ModelState.AddModelError(
                    nameof(request.Status),
                    $"Invalid status transition from {existing.Status} to {request.Status}.");

                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Step 4: BR-01
            // Only check for another Active assignment when transitioning to Active.
            if (existing.Status != AssignmentStatus.Active && request.Status == AssignmentStatus.Active)
            {
                var hasConflict = await _assignmentRepository.HasActiveAssignment(
                    existing.EmployeeId,
                    existing.AssignmentId,
                    cancellationToken);

                if (hasConflict)
                {
                    return Conflict(new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Active assignment conflict",
                        Detail = "The employee already has another active assignment."
                    });
                }
            }

            // Step 5: Update only AssignmentDate and Status.
            // EmployeeId and DepartmentId remain unchanged.
            var updated = await _assignmentRepository.UpdateAssignment(id,request.AssignmentDate,request.Status,cancellationToken);

            // Protect against a concurrent delete.
            if (updated is null)
            {
                return NotFound();
            }

            _logger.LogInformation("Updated employee department assignment with id {id}",id);

            return Ok(ToResponse(updated));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAssignment(int id,CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting employee department assignment with id {id}", id);

            var isDeleted = await _assignmentRepository.DeleteAssignmentIfExist(id,cancellationToken);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        private static EmployeeDepartmentAssignmentDto ToResponse(EmployeeDepartmentAssignment assignment)=> new(
                assignment.AssignmentId,
                assignment.EmployeeId,
                 assignment.DepartmentId,
                assignment.AssignmentDate,
                assignment.Status
            );
        private static bool IsValidStatusTransition(AssignmentStatus current,AssignmentStatus requested)
        {
            // Same status is explicitly allowed as a no-op.
            if (current == requested)
            {
                return true;
            }

            return current switch
            {
                AssignmentStatus.Scheduled =>
                    requested == AssignmentStatus.Active ||
                    requested == AssignmentStatus.Cancelled,

                AssignmentStatus.Active =>
                    requested == AssignmentStatus.Completed ||
                    requested == AssignmentStatus.Cancelled,

                AssignmentStatus.Completed =>
                    false,

                AssignmentStatus.Cancelled =>
                    false,

                _ => false
            };
        }
    }
}

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

        private static EmployeeDepartmentAssignmentDto ToResponse(EmployeeDepartmentAssignment assignment)=> new(
                assignment.AssignmentId,
                assignment.EmployeeId,
                 assignment.DepartmentId,
                assignment.AssignmentDate,
                assignment.Status
            );
        
    }
}

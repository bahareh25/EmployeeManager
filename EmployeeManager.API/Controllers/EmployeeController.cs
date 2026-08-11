using EmployeeManager.Application.Dtos;
using EmployeeManager.Application.Repositories;
using EmployeeManager.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManager.API.Controllers
{
    /// <summary>
    /// Reference implementation of a full CRUD controller.
    /// </summary>
    /// <remarks>
    /// The controller you write for Assignment 02 should follow this shape:
    /// accept and return DTOs (never EF entities), validate foreign keys before
    /// touching the database, and use the status codes documented in the brief.
    ///
    /// Checks run in a fixed order, because the order decides which status code the
    /// client sees when a request breaks more than one thing at once:
    ///   1. Model binding and data annotations - handled automatically by [ApiController], 400
    ///   2. Does the addressed resource exist? - 404
    ///   3. Is the body valid, including foreign keys? - 400
    ///   4. Business rules - 400, or 409 where the rule is about conflicting state
    /// The assignment brief publishes the same order for /api/assignment. Follow it, or
    /// a correct implementation can still return the wrong code for a given request.
    /// </remarks>
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        /// <summary>GET /api/employees - always 200; an empty collection is not an error.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetEmployees(CancellationToken cancellationToken)
        {
            //Returning every row is fine for 100 seeded records, but think about
            //paging and projection before doing this against a real table.
            _logger.LogInformation("Fetching all employees");

            var employees = await _employeeRepository.GetAllEmployees(cancellationToken);

            //An empty list is a valid result for a collection endpoint: 200 with [],
            //not 404. 404 means "this URL identifies nothing", which is not the case here.
            return Ok(employees.Select(ToResponse).ToList());
        }

        /// <summary>GET /api/employees/{id} - 200 when found, 404 when not.</summary>
        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetEmployeeById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching employee with id {id}", id);

            var employee = await _employeeRepository.GetEmployeeById(id, cancellationToken);

            if (employee is null) return NotFound();

            return Ok(ToResponse(employee));
        }

        /// <summary>
        /// POST /api/employees - 201 Created with a Location header, or 400 on invalid input.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateEmployee(
            [FromBody] CreateEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            //[ApiController] has already rejected the request with a 400 if any data
            //annotation on CreateEmployeeRequest failed, so only the rules that need
            //a database lookup are left to check here.
            if (!await _employeeRepository.DepartmentExists(request.DepartmentId, cancellationToken))
            {
                ModelState.AddModelError(
                    nameof(request.DepartmentId),
                    $"Department {request.DepartmentId} does not exist.");

                //Returns 400 with the same RFC 7807 ValidationProblemDetails body that
                //[ApiController] produces for annotation failures, so clients see one
                //consistent error shape.
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var created = await _employeeRepository.CreateEmployee(
                new Employee
                {
                    Name = request.Name,
                    Email = request.Email,
                    DepartmentId = request.DepartmentId
                },
                cancellationToken);

            _logger.LogInformation("Created employee with id {id}", created.Id);

            //201 Created, with a Location header pointing at the new resource.
            return CreatedAtAction(nameof(GetEmployeeById), new { id = created.Id }, ToResponse(created));
        }

        /// <summary>
        /// PUT /api/employees/{id} - 200 with the updated resource, 404 if it does not exist.
        /// </summary>
        [HttpPut]
        [Route("{id:int}")]
        [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateEmployee(
            int id,
            [FromBody] UpdateEmployeeRequest request,
            CancellationToken cancellationToken)
        {
            //Precedence step 1: does the resource exist? A 404 outranks a 400.
            //Validating the body first would answer "your department id is wrong" for a
            //URL that identifies nothing, which tells the client to fix the wrong thing.
            var existing = await _employeeRepository.GetEmployeeById(id, cancellationToken);

            if (existing is null) return NotFound();

            //Precedence step 2: is the body valid?
            if (!await _employeeRepository.DepartmentExists(request.DepartmentId, cancellationToken))
            {
                ModelState.AddModelError(
                    nameof(request.DepartmentId),
                    $"Department {request.DepartmentId} does not exist.");

                //Returns 400 with the same RFC 7807 ValidationProblemDetails body that
                //[ApiController] produces for annotation failures, so clients see one
                //consistent error shape.
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var updated = await _employeeRepository.UpdateEmployee(
                id,
                new Employee
                {
                    Name = request.Name,
                    Email = request.Email,
                    DepartmentId = request.DepartmentId
                },
                cancellationToken);

            //Still possible if the row was deleted between the two calls above.
            if (updated is null) return NotFound();

            _logger.LogInformation("Updated employee with id {id}", id);

            return Ok(ToResponse(updated));
        }

        /// <summary>DELETE /api/employees/{id} - 204 on success, 404 if it does not exist.</summary>
        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteEmployeeById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting employee with id {id}", id);

            var isDeleted = await _employeeRepository.DeleteEmployeeIfExist(id, cancellationToken);

            if (!isDeleted) return NotFound();

            //204 No Content: the delete succeeded and there is no body to return.
            return NoContent();
        }

        //Mapping entity -> DTO in one place keeps the controller actions readable.
        //On a larger project this belongs in a dedicated mapper.
        private static EmployeeResponse ToResponse(Employee employee) =>
            new(employee.Id, employee.Name, employee.Email, employee.DepartmentId);
    }
}

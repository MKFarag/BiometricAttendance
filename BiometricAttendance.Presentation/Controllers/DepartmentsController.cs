using BiometricAttendance.Application.Contracts.Departments;
using BiometricAttendance.Application.Features.Departments.Add;
using BiometricAttendance.Application.Features.Departments.Get;
using BiometricAttendance.Application.Features.Departments.GetAll;
using BiometricAttendance.Application.Features.Departments.Remove;
using BiometricAttendance.Application.Features.Departments.Update;

namespace BiometricAttendance.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class DepartmentsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves all departments.
    /// </summary>
    /// <remarks>
    /// Returns a list of departments.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of departments.</returns>
    /// <response code="200">Returns the list of departments.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read departments.</response>
    [HttpGet("")]
    [HasPermission(Permissions.ReadDepartment)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllDepartmentsQuery(), cancellationToken));

    /// <summary>
    /// Retrieves a specific department by its ID.
    /// </summary>
    /// <remarks>
    /// Returns the details of a department based on the provided ID.
    /// </remarks>
    /// <param name="id">The Id of the department.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The department details.</returns>
    /// <response code="200">Returns the department details.</response>
    /// <response code="404">If the department is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read department.</response>
    [HttpGet("{id}")]
    [HasPermission(Permissions.ReadDepartment)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDepartmentsQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Creates a new department.
    /// </summary>
    /// <remarks>
    /// Creates a new department.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/Departments
    ///     {
    ///       "name": "CS"
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The department creation request containing name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created department details.</returns>
    /// <response code="201">Returns the newly created department.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to add roles.</response>
    /// <response code="409">If a department with the same name already exists.</response>
    [HttpPost("")]
    [HasPermission(Permissions.AddDepartment)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add([FromBody] DepartmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddDepartmentCommand(request.Name), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Updates an existing department.
    /// </summary>
    /// <remarks>
    /// Updates the details of an existing department by ID.
    /// 
    /// Sample request:
    /// 
    ///     PUT /api/Departments/1
    ///     {
    ///       "name": "Computer Science"
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">The id of the department to update.</param>
    /// <param name="request">The updated department data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the department was successfully updated.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the department is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update departments.</response>
    /// <response code="409">If a department with the same name already exists.</response>
    [HttpPut("{id}")]
    [HasPermission(Permissions.AddDepartment)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DepartmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateDepartmentCommand(id, request.Name), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Deletes a department.
    /// </summary>
    /// <remarks>
    /// Deletes a department by ID if it is not in use.
    /// </remarks>
    /// <param name="id">The id of the department to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the department was successfully deleted.</response>
    /// <response code="404">If the department is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to remove departments.</response>
    /// <response code="409">If the department is in use and cannot be deleted.</response>
    [HttpDelete("{id}")]
    [HasPermission(Permissions.AddDepartment)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remove([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RemoveDepartmentCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}

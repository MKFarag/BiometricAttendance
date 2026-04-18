using BiometricAttendance.Application.Contracts.Students;
using BiometricAttendance.Application.Features.Students.EnrollCourses;
using BiometricAttendance.Application.Features.Students.ChangeDepartment;
using BiometricAttendance.Application.Features.Students.CompleteRegistration;
using BiometricAttendance.Application.Features.Students.GetAll;
using BiometricAttendance.Application.Features.Students.Promote;
using BiometricAttendance.Application.Features.Students.ChangeLevel;
using BiometricAttendance.Application.Features.Students.ForceRemove;

namespace BiometricAttendance.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Sliding)]
public class StudentsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Completes the current student's registration data.
    /// </summary>
    /// <remarks>
    /// Assigns the student role and saves level and department data for the current authenticated user.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/Students/complete-registration
    ///     {
    ///       "level": 2,
    ///       "departmentId": 1
    ///     }
    ///     
    /// </remarks>
    /// <param name="request">The registration completion request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If registration was completed successfully.</response>
    /// <response code="400">If request data is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the user or the department is not found.</response>
    [HttpPost("complete-registration")]
    [Authorize(Roles = DefaultRoles.Pending.Name)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteRegistration([FromBody] CompleteStudentRegistrationRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CompleteStudentRegistrationCommand(User.GetId()!, request), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    /// <summary>
    /// Retrieves all students.
    /// </summary>
    /// <remarks>
    /// Returns a paginated list of students based on the provided filter criteria.
    /// </remarks>
    /// <param name="filters">Filter and pagination options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of students.</returns>
    /// <response code="200">Returns the filtered list of students.</response>
    /// <response code="401">If the user is unauthorized.</response>
    [HttpGet("")]
    [HasPermission(Permissions.ReadStudent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll([FromQuery] RequestFilters filters, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllStudentsQuery(filters), cancellationToken));

    /// <summary>
    /// Adds selected courses for the current student.
    /// </summary>
    /// <remarks>
    /// Allows the authenticated student to enroll in courses that belong to their department.
    ///
    /// Sample request:
    /// 
    ///     POST /api/Students/complete-registration
    ///     {
    ///       "coursesId": [1, 2]
    ///     }
    ///     
    /// </remarks>
    /// <param name="request">The request containing the list of course IDs to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the courses were added successfully.</response>
    /// <response code="400">If one or more courses are invalid for the student's department.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the user is not found.</response>
    [HttpPost("courses")]
    [Authorize(Roles = DefaultRoles.Student.Name)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnrollCourses([FromBody] StudentCoursesRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new EnrollStudentCoursesCommand(User.GetId()!, request.CoursesId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Changes a student's department.
    /// </summary>
    /// <remarks>
    /// Admins can move a student from one department to another existing department.    
    /// Sample request:
    /// 
    ///     PUT /api/Students/1/change-department
    ///     {
    ///       "departmentId": 2
    ///     }
    /// </remarks>
    /// <param name="id">The id of the student.</param>
    /// <param name="request">The request containing the target department ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the student's department was changed successfully.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the student or department is not found.</response>
    [HttpPut("{id}/change-department")]
    [HasPermission(Permissions.ChangeStudentDepartment)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeDepartment([FromRoute] int id, [FromBody] ChangeStudentDepartmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ChangeStudentDepartmentCommand(id, request.DepartmentId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Changes a student's level.
    /// </summary>
    /// <remarks>
    /// Admins can move a student from one department to another existing department.    
    /// Sample request:
    /// 
    ///     PUT /api/Students/1/change-level
    ///     {
    ///       "level": 2
    ///     }
    /// </remarks>
    /// <param name="id">The id of the student.</param>
    /// <param name="request">The request containing the target level.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the student's level was changed successfully.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the student is not found.</response>
    [HttpPut("{id}/change-level")]
    [HasPermission(Permissions.ChangeStudentDepartment)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeLevel([FromRoute] int id, [FromBody] ChangeStudentLevelRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ChangeStudentLevelCommand(id, request.Level), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Promotes a student to the next level.
    /// </summary>
    /// <remarks>
    /// Admins can promote a student when the student has not reached the maximum level.
    /// </remarks>
    /// <param name="id">The id of the student.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the student was promoted successfully.</response>
    /// <response code="400">If the student cannot be promoted.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the student is not found.</response>
    [HttpPut("{id}/promote")]
    [HasPermission(Permissions.PromoteStudent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Promote([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new PromoteStudentCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Force removes a student from the system while keeping his user account.
    /// </summary>
    /// <remarks>
    /// Admins can force remove a student at any time.
    /// </remarks>
    /// <param name="id">The id of the student.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="200">If the student was force removed successfully.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the student is not found.</response>
    [HttpDelete("{id}/force-remove")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ForceRemove([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ForceRemoveStudentCommand(id), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}

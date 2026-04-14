using BiometricAttendance.Application.Contracts.Courses;
using BiometricAttendance.Application.Features.Courses.Add;
using BiometricAttendance.Application.Features.Courses.Get;
using BiometricAttendance.Application.Features.Courses.GetAll;
using BiometricAttendance.Application.Features.Courses.Remove;
using BiometricAttendance.Application.Features.Courses.Update;
using BiometricAttendance.Presentation;

namespace BiometricAttendance.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class CoursesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves all courses.
    /// </summary>
    /// <remarks>
    /// Returns a list of courses.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of courses.</returns>
    /// <response code="200">Returns the list of courses.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read courses.</response>
    [HttpGet("")]
    [HasPermission(Permissions.ReadCourse)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetAllCoursesQuery(), cancellationToken));

    /// <summary>
    /// Retrieves a specific course by its ID.
    /// </summary>
    /// <remarks>
    /// Returns the details of a course based on the provided ID.
    /// </remarks>
    /// <param name="id">The Id of the course.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The course details.</returns>
    /// <response code="200">Returns the course details.</response>
    /// <response code="404">If the course is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to read courses.</response>
    [HttpGet("{id}")]
    [HasPermission(Permissions.ReadCourse)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCourseQuery(id), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <remarks>
    /// Creates a new course.
    ///
    /// Sample request:
    ///
    ///     POST /api/Courses
    ///     {
    ///       "name": "Mathematics",
    ///       "code": "MATH101",
    ///       "level": 1
    ///     }
    ///
    /// </remarks>
    /// <param name="request">The course creation request containing name, code and level.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created course details.</returns>
    /// <response code="201">Returns the newly created course.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to add courses.</response>
    /// <response code="409">If a course with the same name or code already exists.</response>
    [HttpPost("")]
    [HasPermission(Permissions.AddCourse)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add([FromBody] CourseRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new AddCourseCommand(request.Name, request.Code, request.Level), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    /// <remarks>
    /// Updates the details of an existing course by ID.
    ///
    /// Sample request:
    ///
    ///     PUT /api/Courses/1
    ///     {
    ///       "name": "Advanced Mathematics",
    ///       "code": "MATH201",
    ///       "level": 2
    ///     }
    ///
    /// </remarks>
    /// <param name="id">The id of the course to update.</param>
    /// <param name="request">The updated course data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the course was successfully updated.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the course is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to update courses.</response>
    /// <response code="409">If a course with the same name or code already exists.</response>
    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateCourse)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateCourseCommand(id, request.Name, request.Code, request.Level), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Deletes a course.
    /// </summary>
    /// <remarks>
    /// Deletes a course by ID if it is not in use.
    /// </remarks>
    /// <param name="id">The id of the course to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the course was successfully deleted.</response>
    /// <response code="404">If the course is not found.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user does not have permission to remove courses.</response>
    /// <response code="409">If the course is in use and cannot be deleted.</response>
    [HttpDelete("{id}")]
    [HasPermission(Permissions.RemoveCourse)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remove([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RemoveCourseCommand(id), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}

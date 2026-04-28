using BiometricAttendance.Application.Contracts.Attendances;
using BiometricAttendance.Application.Features.Attendances.GetMyAttendance;
using BiometricAttendance.Application.Features.Attendances.GetStudentAttendanceDetail;
using BiometricAttendance.Application.Features.Attendances.GetTotalAttendance;
using BiometricAttendance.Application.Features.Attendances.GetWeekAttendance;
using BiometricAttendance.Application.Features.Attendances.MarkAttendance;

namespace BiometricAttendance.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Sliding)]
public class AttendancesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Retrieves the current authenticated student's attendance across all enrolled courses.
    /// </summary>
    /// <remarks>
    /// Returns a per-course attendance breakdown with individual and overall attendance percentages for the authenticated student.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The student's attendance summary.</returns>
    /// <response code="200">Returns the attendance summary.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="404">If the student profile is not found.</response>
    [HttpGet("my")]
    [Authorize(Roles = DefaultRoles.Student.Name)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyAttendance(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyAttendanceQuery(User.GetId()!), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Retrieves attendance records for a specific course week.
    /// </summary>
    /// <remarks>
    /// Returns all students whose attendance was recorded for the selected course and week number.
    /// </remarks>
    /// <param name="courseId">The id of the course.</param>
    /// <param name="weekNumber">The week number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The attendance records for the selected week.</returns>
    /// <response code="200">Returns the week attendance records.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not allowed to read attendance.</response>
    /// <response code="404">If the course is not found.</response>
    [HttpGet("courses/{courseId}/weeks/{weekNumber}")]
    [HasPermission(Permissions.ReadAttendance)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWeekAttendance([FromRoute] int courseId, [FromRoute] int weekNumber, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetWeekAttendanceQuery(courseId, weekNumber), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Retrieves the total attendance percentage for each student in a course.
    /// </summary>
    /// <remarks>
    /// Returns one record per student with the calculated attendance percentage based on all recorded weeks for the course.
    /// </remarks>
    /// <param name="courseId">The id of the course.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total attendance summary for the course.</returns>
    /// <response code="200">Returns the total attendance summary.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not allowed to read attendance.</response>
    /// <response code="404">If the course is not found.</response>
    [HttpGet("courses/{courseId}/total")]
    [HasPermission(Permissions.ReadAttendance)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTotalAttendance([FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTotalAttendanceQuery(courseId), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Retrieves the attendance details of a specific student in a course.
    /// </summary>
    /// <remarks>
    /// Returns the student information together with the calculated total attendance percentage for the selected course.
    /// </remarks>
    /// <param name="studentId">The id of the student.</param>
    /// <param name="courseId">The id of the course.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The student attendance details.</returns>
    /// <response code="200">Returns the attendance details.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not allowed to read attendance.</response>
    /// <response code="404">If the student or course is not found.</response>
    [HttpGet("students/{studentId}/courses/{courseId}")]
    [HasPermission(Permissions.ReadAttendance)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentAttendanceDetail([FromRoute] int studentId, [FromRoute] int courseId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetStudentAttendanceDetailQuery(studentId, courseId), cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Marks attendance manually for a student.
    /// </summary>
    /// <remarks>
    /// Creates a single attendance record for the specified student, course, and week number.
    ///
    /// Sample request:
    ///
    ///     POST /api/Attendances/mark
    ///     {
    ///       "studentId": 1,
    ///       "courseId": 1,
    ///       "weekNumber": 3
    ///     }
    ///
    /// </remarks>
    /// <param name="request">The request containing the student id, course id, and week number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If attendance was marked successfully.</response>
    /// <response code="400">If the student is not enrolled in the selected course.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not allowed to manage attendance.</response>
    /// <response code="404">If the student or course is not found.</response>
    /// <response code="409">If attendance was already recorded for the same week.</response>
    [HttpPost("mark")]
    [HasPermission(Permissions.MarkAttendance)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new MarkAttendanceCommand(request.StudentId, request.CourseId, request.WeekNumber), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}

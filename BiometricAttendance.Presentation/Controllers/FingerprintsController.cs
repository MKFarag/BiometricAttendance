using BiometricAttendance.Application.Contracts.Attendances;
using BiometricAttendance.Application.Features.Fingerprints.EndTakingAttendance;
using BiometricAttendance.Application.Features.Fingerprints.Register;
using BiometricAttendance.Application.Features.Fingerprints.StartTakingAttendance;

namespace BiometricAttendance.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
public class FingerprintsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Starts fingerprint registration for a student.
    /// </summary>
    /// <remarks>
    /// Queues a fingerprint enrollment job for the specified student if the student exists and does not already have a fingerprint.
    /// </remarks>
    /// <param name="studentId">The id of the student.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If fingerprint registration was queued successfully.</response>
    /// <response code="400">If the student id is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not allowed to manage fingerprints.</response>
    /// <response code="404">If the student is not found.</response>
    /// <response code="409">If the student already has a fingerprint.</response>
    [HttpPost("register/{studentId}")]
    [HasPermission(Permissions.FingerprintRegister)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromRoute] int studentId, CancellationToken cancellationToken)
    {
        if (studentId <= 0)
            return BadRequest();

        var result = await _sender.Send(new FingerprintRegisterCommand(studentId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    /// <summary>
    /// Starts a fingerprint-based attendance session.
    /// </summary>
    /// <remarks>
    /// Starts collecting fingerprint identifiers from the device so they can be recorded later for a course week.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If attendance collection started successfully.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not allowed to manage fingerprints.</response>
    [HttpPost("attendance/start")]
    [HasPermission(Permissions.FingerprintAction)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> StartTakingAttendance(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new StartTakingAttendanceQuery(), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    /// <summary>
    /// Ends a fingerprint-based attendance session and saves the collected records.
    /// </summary>
    /// <remarks>
    /// Stops the active attendance session, matches collected fingerprint IDs to enrolled students in the selected course,
    /// and stores the attendance for the provided week number.
    ///
    /// Sample request:
    ///
    ///     POST /api/Fingerprints/attendance/end
    ///     {
    ///       "courseId": 1,
    ///       "weekNum": 3
    ///     }
    ///
    /// </remarks>
    /// <param name="request">The request containing the course id and week number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If the attendance session ended successfully.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not allowed to manage fingerprints.</response>
    /// <response code="404">If the course is not found.</response>
    /// <response code="409">If attendance for the selected week was already recorded.</response>
    /// <response code="503">If no active attendance session is running.</response>
    [HttpPost("attendance/end")]
    [HasPermission(Permissions.FingerprintAction)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> EndTakingAttendance([FromBody] EndTakingAttendanceRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new EndTakingAttendanceCommand(request.CourseId, request.WeekNum), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}

//using BiometricAttendance.Application.Features.Fingerprint.CheckEnrollmentState;
//using BiometricAttendance.Application.Features.Fingerprint.DeleteAllData;
//using BiometricAttendance.Application.Features.Fingerprint.EndAttendance;
//using BiometricAttendance.Application.Features.Fingerprint.GetLastData;
//using BiometricAttendance.Application.Features.Fingerprint.Match;
//using BiometricAttendance.Application.Features.Fingerprint.SetEnrollmentState;
//using BiometricAttendance.Application.Features.Fingerprint.StartAttendance;
//using BiometricAttendance.Application.Features.Fingerprint.StartEnrollment;
//using BiometricAttendance.Application.Features.Fingerprint.StartReader;
//using BiometricAttendance.Application.Features.Fingerprint.StopReader;

//namespace BiometricAttendance.Presentation.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//[Authorize]
//[EnableRateLimiting(RateLimitingOptions.PolicyNames.Concurrency)]
//public class FingerprintController(ISender sender) : ControllerBase
//{
//    private readonly ISender _sender = sender;

//    #region Diagnostic / Hardware Control

//    /// <summary>Opens the serial port and starts the fingerprint reader.</summary>
//    /// <response code="200">Reader started successfully.</response>
//    /// <response code="409">Reader is already running.</response>
//    /// <response code="500">Failed to open the serial port.</response>
//    [HttpPost("start")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status409Conflict)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    public async Task<IActionResult> Start(CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new StartReaderCommand(), cancellationToken);

//        return result.IsSuccess ? Ok() : result.ToProblem();
//    }

//    /// <summary>Closes the serial port and stops the fingerprint reader.</summary>
//    /// <response code="200">Reader stopped successfully.</response>
//    /// <response code="503">Reader was not running.</response>
//    [HttpPost("stop")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
//    public async Task<IActionResult> Stop(CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new StopReaderCommand(), cancellationToken);

//        return result.IsSuccess ? Ok() : result.ToProblem();
//    }

//    /// <summary>Enables or disables enrollment mode on the reader.</summary>
//    /// <param name="allow">True to allow enrollment, false to deny.</param>
//    /// <response code="200">Command sent successfully.</response>
//    /// <response code="503">Reader is not running.</response>
//    [HttpPost("enrollment-state")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
//    public async Task<IActionResult> SetEnrollmentState([FromQuery] bool allow, CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new SetEnrollmentStateCommand(allow), cancellationToken);

//        return result.IsSuccess ? Ok() : result.ToProblem();
//    }

//    /// <summary>Queries the reader to check whether enrollment is currently allowed.</summary>
//    /// <response code="200">Returns true (allowed) or false (denied).</response>
//    /// <response code="408">No response within the timeout.</response>
//    /// <response code="503">Reader is not running.</response>
//    [HttpGet("enrollment-allowed")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
//    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
//    public async Task<IActionResult> IsEnrollmentAllowed(CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new CheckEnrollmentStateQuery(), cancellationToken);

//        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
//    }

//    /// <summary>Returns the last raw string received over the serial port.</summary>
//    /// <response code="200">Returns the last received data.</response>
//    /// <response code="404">No data has been received yet.</response>
//    /// <response code="503">Reader is not running.</response>
//    [HttpGet("last-data")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
//    public async Task<IActionResult> GetLastData(CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new GetLastFingerprintDataQuery(), cancellationToken);

//        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
//    }

//    #endregion

//    #region Main Operations

//    /// <summary>
//    /// Sends the enrollment start command and queues a background job that waits for the reader
//    /// to return a fingerprint ID and links it to the student.
//    /// </summary>
//    /// <param name="studentId">The student to enrol.</param>
//    /// <response code="200">Enrollment started — background job queued.</response>
//    /// <response code="404">Student not found.</response>
//    /// <response code="503">Reader failed to start.</response>
//    // TODO: Fully functional once IStudentService.RegisterFingerprintAsync is implemented
//    [HttpPost("enrollment/{studentId:int}")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
//    public async Task<IActionResult> StartEnrollment([FromRoute] int studentId, CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new StartEnrollmentCommand(studentId), cancellationToken);

//        return result.IsSuccess ? Ok() : result.ToProblem();
//    }

//    /// <summary>
//    /// Sends the delete command to the reader and removes all fingerprint associations from the database.
//    /// Requires the delete password defined in EnrollmentCommands configuration.
//    /// </summary>
//    /// <param name="password">Must match the configured delete command string.</param>
//    /// <response code="204">All data deleted.</response>
//    /// <response code="403">Wrong password.</response>
//    /// <response code="503">Reader failed to start.</response>
//    // TODO: Fully functional once IStudentService.RemoveAllFingerprintsAsync is implemented
//    [HttpDelete("data")]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(StatusCodes.Status403Forbidden)]
//    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
//    public async Task<IActionResult> DeleteAllData([FromQuery] string password, CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new DeleteAllFingerprintDataCommand(password), cancellationToken);

//        return result.IsSuccess ? NoContent() : result.ToProblem();
//    }

//    /// <summary>
//    /// Reads the latest detected fingerprint ID and resolves it to a matched student.
//    /// </summary>
//    /// <response code="200">Returns the matched student.</response>
//    /// <response code="404">No data available or no student matched.</response>
//    /// <response code="503">Reader is not running.</response>
//    // TODO: Fully functional once IStudentService.GetAsync is implemented
//    [HttpGet("match")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
//    public async Task<IActionResult> Match(CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new MatchFingerprintQuery(), cancellationToken);

//        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
//    }

//    /// <summary>
//    /// Starts continuous fingerprint reading for an attendance session.
//    /// Collected IDs are held in session state until EndAttendance is called.
//    /// </summary>
//    /// <response code="200">Attendance reading started.</response>
//    /// <response code="503">Reader failed to start.</response>
//    [HttpPost("attendance/start")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
//    public async Task<IActionResult> StartAttendance(CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new StartAttendanceCommand(), cancellationToken);

//        return result.IsSuccess ? Ok() : result.ToProblem();
//    }

//    /// <summary>
//    /// Stops the attendance session and records attendance for all collected fingerprints.
//    /// </summary>
//    /// <param name="courseId">The course ID to record attendance against.</param>
//    /// <param name="weekNum">The week number (1–12).</param>
//    /// <response code="200">Attendance recorded.</response>
//    /// <response code="400">Invalid week number or no fingerprints collected.</response>
//    /// <response code="404">Course not found.</response>
//    /// <response code="503">No active attendance session.</response>
//    // TODO: Fully functional once IStudentService.GetAllIdsAsync and IAttendanceService are implemented
//    [HttpPost("attendance/end")]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
//    public async Task<IActionResult> EndAttendance([FromQuery] int courseId, [FromQuery] int weekNum, CancellationToken cancellationToken)
//    {
//        var result = await _sender.Send(new EndAttendanceCommand(courseId, weekNum), cancellationToken);

//        return result.IsSuccess ? Ok() : result.ToProblem();
//    }

//    #endregion
//}

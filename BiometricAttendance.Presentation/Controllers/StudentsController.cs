using BiometricAttendance.Application.Contracts.Students;
using BiometricAttendance.Application.Features.Students.CompleteRegistration;

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
    /// </remarks>
    /// <param name="request">The registration completion request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If registration was completed successfully.</response>
    /// <response code="400">If request data is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    [HttpPost("complete-registration")]
    [Authorize(Roles = DefaultRoles.Pending.Name)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CompleteRegistration([FromBody] CompleteStudentRegistrationRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CompleteStudentRegistrationCommand(User.GetId()!, request), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}

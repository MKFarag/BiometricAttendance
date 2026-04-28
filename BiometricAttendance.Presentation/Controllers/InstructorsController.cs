using BiometricAttendance.Application.Contracts.Auth;
using BiometricAttendance.Application.Features.Instructors.GetRole;
using BiometricAttendance.Application.Features.Instructors.GetPass;

namespace BiometricAttendance.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimitingOptions.PolicyNames.Sliding)]
public class InstructorsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    /// <summary>
    /// Assigns instructor role to the current authenticated user.
    /// </summary>
    /// <remarks>
    /// Validates the instructor pass and updates the current user's role to instructor.
    /// </remarks>
    /// <param name="request">The request containing instructor pass.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If role was assigned successfully.</response>
    /// <response code="400">If pass is invalid.</response>
    /// <response code="401">If the user is unauthorized.</response>
    [HttpPost("role")]
    [Authorize(Roles = DefaultRoles.Pending.Name)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRole([FromBody] GetInstructorRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetInstructorRoleCommand(User.GetId()!, request.Pass), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    /// <summary>
    /// Get the current instructor password.
    /// </summary>
    /// <remarks>
    /// Returns the current available instructor password.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ok on success.</returns>
    /// <response code="200">If the password returns successfully.</response>
    /// <response code="404">If there is no available password.</response>
    /// <response code="401">If the user is unauthorized.</response>
    [HttpGet("pass")]
    [Authorize(Roles = DefaultRoles.Pending.Name)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRole(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetInstructorPassQuery(), cancellationToken);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}

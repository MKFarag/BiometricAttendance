using BiometricAttendance.Application.Features.Fingerprints.Register;

namespace BiometricAttendance.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FingerprintsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost("register/{studentId}")]
    public async Task<IActionResult> Register([FromRoute] int studentId, CancellationToken cancellationToken)
    {
        if (studentId <= 0)
            return BadRequest();

        var result = await _sender.Send(new FingerprintRegisterCommand(studentId), cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}

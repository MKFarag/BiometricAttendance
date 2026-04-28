namespace BiometricAttendance.Application.Features.Fingerprints.StartTakingAttendance;

public class StartTakingAttendanceQueryHandler(IJobManager jobManager, IFingerprintService fingerprintService, FingerprintStatus fingerprintStatus) : IRequestHandler<StartTakingAttendanceQuery, Result>
{
    private readonly IJobManager _jobManager = jobManager;
    private readonly IFingerprintService _fingerprintService = fingerprintService;
    private readonly FingerprintStatus _fingerprintStatus = fingerprintStatus;

    public async Task<Result> Handle(StartTakingAttendanceQuery request, CancellationToken cancellationToken = default)
    {
        if (_fingerprintStatus.IsAttendanceActionWorking)
            return Result.Failure(FingerprintErrors.AttendanceActionAlreadyWorking);

        _fingerprintStatus.StartAttendanceAction();

        _jobManager.Enqueue(() => _fingerprintService.ExecuteStartAttendanceJob());

        return Result.Success();
    }
}

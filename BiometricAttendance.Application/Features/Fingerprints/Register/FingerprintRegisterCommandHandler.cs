namespace BiometricAttendance.Application.Features.Fingerprints.Register;

public class FingerprintRegisterCommandHandler(IUnitOfWork unitOfWork, IJobManager jobManager) : IRequestHandler<FingerprintRegisterCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJobManager _jobManager = jobManager;

    public async Task<Result> Handle(FingerprintRegisterCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Students.AnyAsync(x => x.Id == request.StudentId, cancellationToken))
            return Result.Failure(StudentErrors.NotFound);

        _jobManager.Enqueue<IFingerprintService>(x => x.ExecuteEnrollmentJob(request.StudentId));

        return Result.Success();
    }
}

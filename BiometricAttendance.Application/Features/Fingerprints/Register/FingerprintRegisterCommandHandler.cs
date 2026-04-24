namespace BiometricAttendance.Application.Features.Fingerprints.Register;

public class FingerprintRegisterCommandHandler(IUnitOfWork unitOfWork, IJobManager jobManager) : IRequestHandler<FingerprintRegisterCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJobManager _jobManager = jobManager;

    public async Task<Result> Handle(FingerprintRegisterCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync([request.StudentId], cancellationToken) is not { } student)
            return Result.Failure(StudentErrors.NotFound);

        if (student.FingerprintId.HasValue)
            return Result.Failure(StudentErrors.HasFingerprintId);

        _jobManager.Enqueue<IFingerprintService>(x => x.ExecuteEnrollmentJob(student.Id));

        return Result.Success();
    }
}

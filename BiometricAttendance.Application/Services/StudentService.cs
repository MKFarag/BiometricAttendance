namespace BiometricAttendance.Application.Services;

public class StudentService(IUnitOfWork unitOfWork) : IStudentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task SetFingerprintAsync(int studentId, int fingerprintId, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync([studentId], cancellationToken) is not { } student)
            throw new InvalidOperationException("No student found by this id.");

        if (student.FingerprintId.HasValue)
            throw new InvalidOperationException("This student already has fingerprintId.");

        var fingerprint = Fingerprint.Create(fingerprintId, student.Id);

        await _unitOfWork.Fingerprints.AddAsync(fingerprint, cancellationToken);

        student.SetFingerprint(fingerprint);

        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}

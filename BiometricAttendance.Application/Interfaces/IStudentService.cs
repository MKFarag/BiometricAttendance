namespace BiometricAttendance.Application.Interfaces;

public interface IStudentService
{
    Task SetFingerprintAsync(int studentId, int fingerprintId, CancellationToken cancellationToken = default);
}

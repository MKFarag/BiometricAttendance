namespace BiometricAttendance.Application.Interfaces;

public interface IEnrollmentService
{
    Task<Result> StartEnrollmentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAllDataAsync(string password);
    Result SetEnrollmentState(bool allowEnrollment);
    Task<Result<bool>> IsEnrollmentAllowedAsync(CancellationToken cancellationToken = default);
}

namespace BiometricAttendance.Application.Interfaces;

/// <summary>
/// Background job methods invoked by Hangfire.
/// Must stay as an interface so Application-layer handlers can enqueue jobs
/// without a direct dependency on the Infrastructure class.
/// </summary>
public interface IFingerprintJobService
{
    Task ExecuteEnrollmentJob(int studentId);
    Task ExecuteStartAttendanceJob();
}

namespace BiometricAttendance.Application.Interfaces;

public interface IFingerprintService
{
    Task ExecuteEnrollmentJob(int studentId);
    Task ExecuteStartAttendanceJob();
}
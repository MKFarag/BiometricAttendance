namespace BiometricAttendance.Application.Interfaces;

public interface IInstructorPassService
{
    Task<bool> TryUse(string userId, string pass, CancellationToken cancellationToken = default);
    Task CreateNewPass(CancellationToken cancellationToken = default);
}

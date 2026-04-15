namespace BiometricAttendance.Application.Interfaces;

public interface IInstructorPassService
{
    Task<bool> TryUseAsync(string userId, string pass, CancellationToken cancellationToken = default);
    Task CreateNewPassAsync(CancellationToken cancellationToken = default);
}

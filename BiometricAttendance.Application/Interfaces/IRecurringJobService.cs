namespace BiometricAttendance.Application.Interfaces;

public interface IRecurringJobService
{
    Task RemoveExpiredRefreshTokenAsync();
}

namespace BiometricAttendance.Application.Services;

public class RecurringJobService(IUnitOfWork unitOfWork) : IRecurringJobService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task RemoveExpiredRefreshTokenAsync()
    {
        await _unitOfWork.Users.RemoveExpiredRefreshTokensAsync();
    }
}

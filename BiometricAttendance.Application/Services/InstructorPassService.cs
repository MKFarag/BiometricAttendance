namespace BiometricAttendance.Application.Services;

public class InstructorPassService(IUnitOfWork unitOfWork) : IInstructorPassService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private const int _maxUses = 10;

    public async Task<bool> TryUse(string userId, string pass, CancellationToken cancellationToken = default)
    {
        var currentPass = await _unitOfWork.InstructorPasses
            .TrackedFindAsync(x => !x.IsDisabled && x.UsedBy.Count < x.MaxUsedCount, cancellationToken)
            ?? throw new Exception("Instructor password must have at least one valid pass.");

        if (!string.Equals(pass, currentPass.PassCode))
            return false;

        currentPass.Use(userId);

        if (currentPass.IsExhausted)
            await _unitOfWork.InstructorPasses.AddAsync(new InstructorPass(_maxUses), cancellationToken);
        
        await _unitOfWork.CompleteAsync(cancellationToken);

        return true;
    }

    public async Task CreateNewPass(CancellationToken cancellationToken = default)
    {
        var currentPass = await _unitOfWork.InstructorPasses
            .TrackedFindAsync(x => !x.IsDisabled && x.UsedBy.Count < x.MaxUsedCount, cancellationToken)
            ?? throw new Exception("Instructor password must have at least one valid pass.");

        if (currentPass.GeneratedAt > DateTime.UtcNow.AddHours(-12))
            return;
        else if (currentPass.UsedBy.Count == 0)
            currentPass.Renew();
        else
        {
            currentPass.Disable();
            await _unitOfWork.InstructorPasses.AddAsync(new InstructorPass(_maxUses), cancellationToken);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}

namespace BiometricAttendance.Application.Features.Users.ToggleStatus;

public class ToggleUserStatusCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<ToggleUserStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.Id, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        user.ToggleStatus();

        var result = await _unitOfWork.Users.UpdateAsync(user);

        await _cacheService.RemoveByTagAsync(Cache.Tags.Users, cancellationToken);

        return result;
    }
}

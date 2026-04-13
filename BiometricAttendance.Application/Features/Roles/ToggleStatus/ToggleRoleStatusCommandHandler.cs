namespace BiometricAttendance.Application.Features.Roles.ToggleStatus;

public class ToggleRoleStatusCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<ToggleRoleStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(ToggleRoleStatusCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Roles.GetAsync(request.Id, cancellationToken) is not { } role)
            return Result.Failure(RoleErrors.NotFound);

        role.IsDisabled = !role.IsDisabled;

        var result = await _unitOfWork.Roles.UpdateAsync(role);

        if (result.IsSuccess)
            await _cacheService.RemoveByTagAsync(Cache.Tags.Roles, cancellationToken);

        return result;
    }
}

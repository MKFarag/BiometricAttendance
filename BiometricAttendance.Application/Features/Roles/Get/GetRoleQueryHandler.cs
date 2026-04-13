namespace BiometricAttendance.Application.Features.Roles.Get;

public class GetRoleQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRoleQuery, Result<RoleDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RoleDetailResponse>> Handle(GetRoleQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Roles.GetAsync(request.Id, cancellationToken) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.NotFound);

        var permissions = await _unitOfWork.Roles.GetClaimsAsync(role.Id, Permissions.Type, cancellationToken);

        var response = new RoleDetailResponse(role.Id, role.Name, role.IsDisabled, permissions);

        return Result.Success(response);
    }
}

namespace BiometricAttendance.Application.Features.Roles.Add;

public class AddRoleCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<AddRoleCommand, Result<RoleDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<RoleDetailResponse>> Handle(AddRoleCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Roles.NameExistsAsync(command.Request.Name, cancellationToken))
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAll().ToHashSet();

        if (command.Request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        var role = command.Request.Adapt<Role>();

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _unitOfWork.Roles.CreateAsync(role);

            if (result.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure<RoleDetailResponse>(result.Error);
            }

            await _unitOfWork.Roles.AddClaimsAsync(role.Id, Permissions.Type, command.Request.Permissions, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result.Failure<RoleDetailResponse>(RoleErrors.CreationCanceled);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        await _cacheService.RemoveByTagAsync(Cache.Tags.Roles, cancellationToken);

        var response = new RoleDetailResponse(role.Id, role.Name, role.IsDisabled, command.Request.Permissions);

        return Result.Success(response);
    }
}

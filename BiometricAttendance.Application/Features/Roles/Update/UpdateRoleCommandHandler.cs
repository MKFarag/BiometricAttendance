namespace BiometricAttendance.Application.Features.Roles.Update;

public class UpdateRoleCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(UpdateRoleCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Roles.GetAsync(command.Id, cancellationToken) is not { } role)
            return Result.Failure(RoleErrors.NotFound);

        if (await _unitOfWork.Roles.NameExistsAsync(command.Request.Name, role.Id, cancellationToken))
            return Result.Failure(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAll().ToHashSet();

        if (command.Request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure(RoleErrors.InvalidPermissions);

        role.Update(command.Request.Name);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _unitOfWork.Roles.UpdateAsync(role);

            if (result.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return result;
            }

            var currentPermissions = await _unitOfWork.Roles.GetClaimsAsync(role.Id, cancellationToken);
            var permissionsToAdd = command.Request.Permissions.Except(currentPermissions).ToList();
            var permissionsToRemove = currentPermissions.Except(command.Request.Permissions).ToList();

            if (permissionsToRemove.Count != 0)
                await _unitOfWork.Roles.DeleteClaimsAsync(role.Id, permissionsToRemove, cancellationToken);

            if (permissionsToAdd.Count != 0)
                await _unitOfWork.Roles.AddClaimsAsync(role.Id, Permissions.Type, permissionsToAdd, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result.Failure(RoleErrors.CreationCanceled);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        await _cacheService.RemoveByTagAsync(Cache.Tags.Roles, cancellationToken);

        return Result.Success();
    }
}

namespace BiometricAttendance.Application.Features.Users.Update;

public class UpdateUserCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(command.Id, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        if (!string.Equals(user.Email, command.Request.Email, StringComparison.OrdinalIgnoreCase) &&
            await _unitOfWork.Users.EmailExistsAsync(command.Request.Email, command.Id, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        if (!string.Equals(user.UserName, command.Request.UserName, StringComparison.OrdinalIgnoreCase) &&
            await _unitOfWork.Users.UserNameExistsAsync(command.Request.UserName, command.Id, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedUserName);

        var allowedRoles = await _unitOfWork.Roles.GetAllNamesAsync(false, false, cancellationToken);

        if (command.Request.Roles.Except(allowedRoles).Any())
            return Result.Failure(UserErrors.InvalidRoles);

        user.Update(command.Request.Email, command.Request.UserName, command.Request.FirstName, command.Request.LastName);

        var result = await _unitOfWork.Users.UpdateAsync(user);

        if (result.IsFailure)
            return result;

        var currentRoles = await _unitOfWork.Users.GetRolesAsync(user);
        var rolesToAdd = command.Request.Roles.Except(currentRoles);
        var rolesToRemove = currentRoles.Except(command.Request.Roles);

        if (rolesToAdd.Any() || rolesToRemove.Any())
        {
            await _unitOfWork.Users.DeleteAllRolesAsync(user);
            await _unitOfWork.Users.AddToRolesAsync(user, command.Request.Roles);
        }

        await _cacheService.RemoveByTagAsync(Cache.Tags.Users, cancellationToken);

        return Result.Success();
    }
}

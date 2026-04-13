namespace BiometricAttendance.Application.Features.Users.Add;

public class AddUserCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<AddUserCommand, Result<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<UserResponse>> Handle(AddUserCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(command.Request.Email, cancellationToken))
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        if (await _unitOfWork.Users.UserNameExistsAsync(command.Request.UserName, cancellationToken))
            return Result.Failure<UserResponse>(UserErrors.DuplicatedUserName);

        var allowedRoles = await _unitOfWork.Roles.GetAllNamesAsync(false, false, cancellationToken);

        if (command.Request.Roles.Except(allowedRoles).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = command.Request.Adapt<User>();

        var result = await _unitOfWork.Users.CreateAsync(user, command.Request.Password, true);

        if (result.IsFailure)
            return Result.Failure<UserResponse>(result.Error);

        await _unitOfWork.Users.AddToRolesAsync(user, command.Request.Roles);

        await _cacheService.RemoveByTagAsync(Cache.Tags.Users, cancellationToken);

        var response = (user, command.Request.Roles).Adapt<UserResponse>();

        return Result.Success(response);
    }
}

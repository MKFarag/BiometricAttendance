namespace BiometricAttendance.Application.Features.Profile.ChangeUserName;

public class ChangeMyUserNameCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeMyUserNameCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ChangeMyUserNameCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        if (string.Equals(user.UserName, request.NewUserName, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(UserErrors.SameUserName);

        if (await _unitOfWork.Users.UserNameExistsAsync(request.NewUserName, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedUserName);

        if (!await _unitOfWork.Users.IsChangeUserNameAvailable(user))
            return Result.Failure(UserErrors.UserNameChangeNotAllowed);

        var result = await _unitOfWork.Users.ChangeUserNameAsync(user, request.NewUserName, User.MinDaysBetweenUserNameChanges);

        return result;
    }
}
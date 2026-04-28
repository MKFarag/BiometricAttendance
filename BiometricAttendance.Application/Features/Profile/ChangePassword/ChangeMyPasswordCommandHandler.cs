namespace BiometricAttendance.Application.Features.Profile.ChangePassword;

public class ChangeMyPasswordCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeMyPasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        var result = await _unitOfWork.Users.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        return result;
    }
}

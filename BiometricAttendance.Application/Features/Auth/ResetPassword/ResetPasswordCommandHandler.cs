namespace BiometricAttendance.Application.Features.Auth.ResetPassword;

public class ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null || !await _unitOfWork.Users.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.InvalidToken);

        if (_urlEncoder.Decode(request.Token) is not { } token)
            return Result.Failure(UserErrors.InvalidToken);

        var result = await _unitOfWork.Users.ResetPasswordAsync(user, token, request.NewPassword);

        return result;
    }
}

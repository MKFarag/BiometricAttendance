namespace BiometricAttendance.Application.Features.Auth.SendResetPassword;

public class SendResetPasswordCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder, INotificationService notificationService) : IRequestHandler<SendResetPasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Handle(SendResetPasswordCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByEmailAsync(request.Email, cancellationToken) is not { } user)
            return Result.Success();

        if (!await _unitOfWork.Users.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.EmailNotConfirmed with { StatusCode = StatusCodes.BadRequest });

        var token = await _unitOfWork.Users.GeneratePasswordResetTokenAsync(user);
        token = _urlEncoder.Encode(token);

        await _notificationService.SendResetPasswordAsync(user, token);

        return Result.Success();
    }
}

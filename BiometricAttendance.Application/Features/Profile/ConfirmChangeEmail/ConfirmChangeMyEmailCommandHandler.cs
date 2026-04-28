namespace BiometricAttendance.Application.Features.Profile.ConfirmChangeEmail;

public class ConfirmChangeMyEmailCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder, INotificationService notificationService) : IRequestHandler<ConfirmChangeMyEmailCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Handle(ConfirmChangeMyEmailCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        var oldEmail = user.Email;

        if (_urlEncoder.Decode(request.Token) is not { } token)
            return Result.Failure(UserErrors.InvalidToken);

        var result = await _unitOfWork.Users.ChangeEmailAsync(user, request.NewEmail, token);

        if (result.IsSuccess)
            await _notificationService.SendChangeEmailNotificationAsync(user, oldEmail, DateTime.UtcNow);

        return result;
    }
}

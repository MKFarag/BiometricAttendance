namespace BiometricAttendance.Application.Features.Profile.ChangeEmail;

public class ChangeMyEmailCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder, INotificationService notificationService) : IRequestHandler<ChangeMyEmailCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Handle(ChangeMyEmailCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        if (string.Equals(user.Email, request.NewEmail, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(UserErrors.SameEmail);

        if (await _unitOfWork.Users.EmailExistsAsync(request.NewEmail, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        var token = await _unitOfWork.Users.GenerateChangeEmailTokenAsync(user, request.NewEmail);

        token = _urlEncoder.Encode(token);

        await _notificationService.SendConfirmationLinkAsync(user, token);

        return Result.Success();
    }
}

namespace BiometricAttendance.Application.Features.Auth.ResendConfirmationEmail;

public class ResendConfirmationEmailCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder, INotificationService notificationService) : IRequestHandler<ResendConfirmationEmailCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByEmailAsync(request.Email, cancellationToken) is not { } user)
            return Result.Success();

        if (await _unitOfWork.Users.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var token = await _unitOfWork.Users.GenerateEmailConfirmationTokenAsync(user);
        token = _urlEncoder.Encode(token);

        await _notificationService.SendConfirmationLinkAsync(user, token);

        return Result.Success();
    }
}

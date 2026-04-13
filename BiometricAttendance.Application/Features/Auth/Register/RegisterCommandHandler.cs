namespace BiometricAttendance.Application.Features.Auth.Register;

public class RegisterCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder, INotificationService notificationService) : IRequestHandler<RegisterCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Handle(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(command.Request.Email, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        if (await _unitOfWork.Users.UserNameExistsAsync(command.Request.UserName, cancellationToken))
            return Result.Failure<string>(UserErrors.DuplicatedUserName);

        var user = command.Request.Adapt<User>();

        var result = await _unitOfWork.Users.CreateAsync(user, command.Request.Password);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        var token = await _unitOfWork.Users.GenerateEmailConfirmationTokenAsync(user);
        token = _urlEncoder.Encode(token);

        await _notificationService.SendConfirmationLinkAsync(user, token);

        return Result.Success();
    }

}

namespace BiometricAttendance.Application.Features.Auth.ConfirmEmail;

public class ConfirmEmailCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder) : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.InvalidToken);

        if (await _unitOfWork.Users.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        if (_urlEncoder.Decode(request.Token) is not { } token)
            return Result.Failure(UserErrors.InvalidToken);

        var result = await _unitOfWork.Users.ConfirmEmailWithTokenAsync(user, token);

        if (result.IsSuccess)
            result = await _unitOfWork.Users.AddToRoleAsync(user, DefaultRoles.Pending.Name);

        return result;
    }
}

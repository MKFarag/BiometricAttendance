namespace BiometricAttendance.Application.Features.Auth.GetRefreshToken;

public class GetRefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtProvider jwtProvider) : IRequestHandler<GetRefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<Result<AuthResponse>> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        if (_jwtProvider.ValidateToken(request.Token) is not { } userId)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (await _unitOfWork.Users.FindByIdAsync(userId, cancellationToken) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (await _unitOfWork.Users.IsLockedOutAsync(user))
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        var revokeResult = await _unitOfWork.Users.RevokeRefreshTokenAsync(user, request.RefreshToken, cancellationToken);

        if (revokeResult.IsFailure)
            return Result.Failure<AuthResponse>(revokeResult.Error);

        var (userRoles, userPermissions) = await _unitOfWork.Users.GetRolesAndPermissionsAsync(user, cancellationToken);

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
        var newRefreshToken = RefreshTokenHandler.GenerateNewToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenHandler.ExpiryDays);

        await _unitOfWork.Users.AddRefreshTokenAsync(user, new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        }, cancellationToken);

        var response = new AuthResponse(user.Id, user.Email, user.UserName, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }
}


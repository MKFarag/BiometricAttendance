namespace BiometricAttendance.Application.Features.Auth.GetToken;

public class GetTokenCommandHandler(IUnitOfWork unitOfWork, IJwtProvider jwtProvider, ISignInService signInService) : IRequestHandler<GetTokenCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ISignInService _signInService = signInService;

    public async Task<Result<AuthResponse>> Handle(GetTokenCommand request, CancellationToken cancellationToken = default)
    {
        var result = await _signInService.PasswordSignInAsync(request.Identifier, request.Password, true);

        if (result.IsFailure)
            return Result.Failure<AuthResponse>(result.Error);

        var user = result.Value;

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var (userRoles, userPermissions) = await _unitOfWork.Users.GetRolesAndPermissionsAsync(user, cancellationToken);

        var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

        var refreshToken = RefreshTokenHandler.GenerateNewToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenHandler.ExpiryDays);

        await _unitOfWork.Users.AddRefreshTokenAsync(user, new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        }, cancellationToken);

        var response = new AuthResponse(user.Id, user.Email, user.UserName, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }
}

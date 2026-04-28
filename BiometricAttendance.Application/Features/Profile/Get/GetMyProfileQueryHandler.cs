namespace BiometricAttendance.Application.Features.Profile.Get;

public class GetMyProfileQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMyProfileQuery, Result<UserProfileResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<UserProfileResponse>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure<UserProfileResponse>(UserErrors.NotFound);

        var response = user.Adapt<UserProfileResponse>();

        return Result.Success(response);
    }
}

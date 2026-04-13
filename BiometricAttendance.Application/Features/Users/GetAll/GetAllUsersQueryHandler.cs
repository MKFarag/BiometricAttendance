namespace BiometricAttendance.Application.Features.Users.GetAll;

public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken = default)
    {
        var users = await _cacheService
            .GetOrCreateAsync
            (
                Cache.Keys.UsersWithoutDefaultRole,
                async ct => await _unitOfWork.Users.GetAllProjectionWithRolesAsync<UserResponse>(false, ct),
                Cache.Expirations.VeryLong,
                [Cache.Tags.Users],
                cancellationToken
            );

        return users;
    }
}

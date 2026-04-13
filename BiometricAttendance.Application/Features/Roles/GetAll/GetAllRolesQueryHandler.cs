namespace BiometricAttendance.Application.Features.Roles.GetAll;

public class GetAllRolesQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<IEnumerable<RoleResponse>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken = default)
    {
        var roles = await _cacheService
            .GetOrCreateAsync
            (
                Cache.Keys.RolesWithoutDefault,
                async ct => await _unitOfWork.Roles.GetAllAsync(false, true, ct),
                Cache.Expirations.VeryLong,
                [Cache.Tags.Roles],
                cancellationToken
            );

        var response = roles
            .Where(r => request.IncludeDisabled || !r.IsDisabled)
            .Select(r => new RoleResponse(r.Id, r.Name, r.IsDisabled))
            .ToList();

        return response;
    }
}

namespace BiometricAttendance.Application.Features.Departments.GetAll;

public class GetAllDepartmentsQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<GetAllDepartmentsQuery, IEnumerable<DepartmentResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<IEnumerable<DepartmentResponse>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken = default)
    {
        var departments = await _cacheService
            .GetOrCreateAsync
            (
                Cache.Keys.Departments,
                _unitOfWork.Departments.GetAllAsync,
                Cache.Expirations.VeryLong,
                [Cache.Tags.Departments],
                cancellationToken
            );

        var response = departments.Adapt<IEnumerable<DepartmentResponse>>();

        return response;
    }
}

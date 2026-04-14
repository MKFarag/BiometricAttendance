namespace BiometricAttendance.Application.Features.Courses.GetAll;

public class GetAllCoursesQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<GetAllCoursesQuery, IEnumerable<CourseResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<IEnumerable<CourseResponse>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken = default)
    {
        var courses = await _cacheService
            .GetOrCreateAsync
            (
                Cache.Keys.Courses,
                _unitOfWork.Courses.GetAllAsync,
                Cache.Expirations.VeryLong,
                [Cache.Tags.Courses],
                cancellationToken
            );

        var response = courses.Adapt<IEnumerable<CourseResponse>>();

        return response;
    }
}

namespace BiometricAttendance.Application.Features.Courses.Get;

public class GetCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCourseQuery, Result<CourseDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CourseDetailResponse>> Handle(GetCourseQuery request, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.FindAsync(x => x.Id == request.Id, [nameof(Course.Department)], cancellationToken);

        if (course is null)
            return Result.Failure<CourseDetailResponse>(CourseErrors.NotFound);

        return Result.Success(course.Adapt<CourseDetailResponse>());
    }
}

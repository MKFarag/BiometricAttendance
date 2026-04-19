namespace BiometricAttendance.Application.Features.Courses.Get;

public class GetCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCourseQuery, Result<CourseDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CourseDetailResponse>> Handle(GetCourseQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Courses.GetAsync([request.Id], cancellationToken) is not { } course)
            return Result.Failure<CourseDetailResponse>(CourseErrors.NotFound);

        if (await _unitOfWork.Departments.GetAsync([course.DepartmentId], cancellationToken) is not { } department)
            return Result.Failure<CourseDetailResponse>(DepartmentErrors.NotFound);

        var response = new CourseDetailResponse(course.Id, course.Name, course.Code, course.DepartmentId, department.Adapt<DepartmentResponse>());

        return Result.Success(response);
    }
}

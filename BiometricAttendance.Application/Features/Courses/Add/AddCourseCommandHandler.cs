namespace BiometricAttendance.Application.Features.Courses.Add;

public class AddCourseCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<AddCourseCommand, Result<CourseResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<CourseResponse>> Handle(AddCourseCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Courses.AnyAsync(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken))
            return Result.Failure<CourseResponse>(CourseErrors.NameAlreadyExists);

        if (await _unitOfWork.Courses.AnyAsync(x => string.Equals(x.Code, request.Code, StringComparison.OrdinalIgnoreCase), cancellationToken))
            return Result.Failure<CourseResponse>(CourseErrors.CodeAlreadyExists);

        if (!await _unitOfWork.Departments.AnyAsync(x => x.Id == request.DepartmentId, cancellationToken))
            return Result.Failure<CourseResponse>(DepartmentErrors.NotFound);

        var course = Course.Create(request.Name, request.Code, request.DepartmentId);

        await _unitOfWork.Courses.AddAsync(course, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cacheService.RemoveByTagAsync(Cache.Tags.Courses, cancellationToken);

        return Result.Success(course.Adapt<CourseResponse>());
    }
}

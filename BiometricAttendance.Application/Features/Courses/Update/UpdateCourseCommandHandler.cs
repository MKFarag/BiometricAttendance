namespace BiometricAttendance.Application.Features.Courses.Update;

public class UpdateCourseCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<UpdateCourseCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Courses.GetAsync([request.Id], cancellationToken) is not { } course)
            return Result.Failure(CourseErrors.NotFound);

        bool isNameEqual = string.Equals(request.Name, course.Name, StringComparison.OrdinalIgnoreCase);
        bool isCodeEqual = string.Equals(request.Code, course.Code, StringComparison.OrdinalIgnoreCase);

        if (isNameEqual && isCodeEqual && request.DepartmentId == course.DepartmentId)
            return Result.Success();

        if (!isNameEqual && await _unitOfWork.Courses.AnyAsync(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken))
            return Result.Failure(CourseErrors.NameAlreadyExists);

        if (!isCodeEqual && await _unitOfWork.Courses.AnyAsync(x => string.Equals(x.Code, request.Code, StringComparison.OrdinalIgnoreCase), cancellationToken))
            return Result.Failure(CourseErrors.CodeAlreadyExists);

        course.Update(request.Name, request.Code, request.DepartmentId);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cacheService.RemoveByTagAsync(Cache.Tags.Courses, cancellationToken);

        return Result.Success();
    }
}

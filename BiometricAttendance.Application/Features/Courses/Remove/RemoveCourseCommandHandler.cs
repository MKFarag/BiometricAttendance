namespace BiometricAttendance.Application.Features.Courses.Remove;

public class RemoveCourseCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<RemoveCourseCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(RemoveCourseCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Courses.AnyAsync(x => x.Id == request.Id, cancellationToken))
            return Result.Failure(CourseErrors.NotFound);

        if (await _unitOfWork.StudentCourses.AnyAsync(x => x.CourseId == request.Id, cancellationToken))
            return Result.Failure(CourseErrors.InUse);

        await _unitOfWork.Courses.ExecuteDeleteAsync(x => x.Id == request.Id, cancellationToken);

        await _cacheService.RemoveByTagAsync(Cache.Tags.Courses, cancellationToken);

        return Result.Success();
    }
}

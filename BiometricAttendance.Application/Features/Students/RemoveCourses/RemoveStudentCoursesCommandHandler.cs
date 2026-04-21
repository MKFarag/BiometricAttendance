namespace BiometricAttendance.Application.Features.Students.RemoveCourses;

public class RemoveStudentCoursesCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RemoveStudentCoursesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RemoveStudentCoursesCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync([request.UserId], cancellationToken) is not { } student)
            return Result.Failure(UserErrors.NotFound);

        var studentCoursesId = await _unitOfWork.StudentCourses
            .FindAllProjectionAsync(x => x.StudentId == student.Id, x => x.CourseId, true, cancellationToken);

        if (request.CoursesId.Except(studentCoursesId).Any())
            return Result.Failure(StudentErrors.InvalidCourses);

        await _unitOfWork.StudentCourses
            .ExecuteDeleteAsync(x => request.CoursesId.Contains(x.CourseId) && x.StudentId == student.Id, cancellationToken);

        return Result.Success();
    }
}

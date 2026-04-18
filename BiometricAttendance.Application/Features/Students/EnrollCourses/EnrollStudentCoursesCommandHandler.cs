namespace BiometricAttendance.Application.Features.Students.EnrollCourses;

public class EnrollStudentCoursesCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<EnrollStudentCoursesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(EnrollStudentCoursesCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync([request.UserId], cancellationToken) is not { } student)
            return Result.Failure(UserErrors.NotFound);

        var allowedCoursesId = await _unitOfWork.Courses
            .FindAllProjectionAsync
            (
                x => x.Level == student.Level && x.DepartmentCourses.Any(dc => dc.DepartmentId == student.DepartmentId),
                [nameof(Course.DepartmentCourses)],
                x => x.Id, 
                true, 
                cancellationToken
            );

        if (request.CoursesId.Except(allowedCoursesId).Any())
            return Result.Failure(StudentErrors.InvalidCourses);

        var studentCourses = request.CoursesId.Select(courseId => new StudentCourse { CourseId = courseId, StudentId = student.Id });

        await _unitOfWork.StudentCourses.AddRangeAsync(studentCourses, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}

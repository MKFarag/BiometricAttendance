namespace BiometricAttendance.Application.Features.Students.AddCourses;

public class AddStudentCoursesCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddStudentCoursesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddStudentCoursesCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync([request.UserId], cancellationToken) is not { } student)
            return Result.Failure(UserErrors.NotFound);

        var allowedCoursesId = await _unitOfWork.DepartmentCourses
            .FindAllProjectionAsync(x => x.DepartmentId == student.DepartmentId, x => x.CourseId, true, cancellationToken);

        if (request.CoursesId.Except(allowedCoursesId).Any())
            return Result.Failure(StudentErrors.InvalidCourses);

        var studentCourses = request.CoursesId.Select(courseId => new StudentCourse { CourseId = courseId, StudentId = student.Id });

        await _unitOfWork.StudentCourses.AddRangeAsync(studentCourses, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}

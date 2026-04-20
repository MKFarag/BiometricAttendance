namespace BiometricAttendance.Application.Features.Students.EnrollCourses;

public class EnrollStudentCoursesCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<EnrollStudentCoursesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(EnrollStudentCoursesCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync([request.UserId], cancellationToken) is not { } student)
            return Result.Failure(UserErrors.NotFound);

        var allowedCoursesId = await _unitOfWork.Courses
            .FindAllProjectionAsync(x => x.DepartmentId == student.DepartmentId, x => x.Id, true, cancellationToken);

        if (request.CoursesId.Except(allowedCoursesId).Any())
            return Result.Failure(StudentErrors.InvalidCourses);

        var studentCourses = request.CoursesId.Select(courseId => StudentCourse.Create(student.Id, courseId));

        await _unitOfWork.StudentCourses.AddRangeAsync(studentCourses, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}

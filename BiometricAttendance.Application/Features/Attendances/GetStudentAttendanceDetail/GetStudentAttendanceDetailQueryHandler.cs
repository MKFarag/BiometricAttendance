namespace BiometricAttendance.Application.Features.Attendances.GetStudentAttendanceDetail;

public class GetStudentAttendanceDetailQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetStudentAttendanceDetailQuery, Result<StudentAttendanceDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<StudentAttendanceDetailResponse>> Handle(GetStudentAttendanceDetailQuery request, CancellationToken cancellationToken = default)
    {
        var student = (await _unitOfWork.Students.FindAllWithNameAsync(
            x => x.Id == request.StudentId,
            cancellationToken)).FirstOrDefault();

        if (student is null)
            return Result.Failure<StudentAttendanceDetailResponse>(StudentErrors.NotFound);

        var course = await _unitOfWork.Courses.FindAsync(x => x.Id == request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure<StudentAttendanceDetailResponse>(CourseErrors.NotFound);

        var allCourseAttendances = (await _unitOfWork.Attendances.FindAllAsync(
            x => x.CourseId == request.CourseId,
            cancellationToken)).ToList();

        var totalWeeks = allCourseAttendances.Select(a => a.WeekNumber).Distinct().Count();

        var studentAttendedWeeks = allCourseAttendances.Count(a => a.StudentId == request.StudentId);

        var percentage = totalWeeks > 0
            ? $"{(double)studentAttendedWeeks / totalWeeks * 100:F2}%"
            : "0.00%";

        var studentResponse = new StudentResponse(student.Id, student.Name ?? student.UserId, student.Level, student.DepartmentName ?? string.Empty);
        var courseAttendanceResponse = new CourseAttendanceResponse(course.Id, course.Name, course.Code, percentage);

        return Result.Success(new StudentAttendanceDetailResponse(student.Id, studentResponse, courseAttendanceResponse));
    }
}

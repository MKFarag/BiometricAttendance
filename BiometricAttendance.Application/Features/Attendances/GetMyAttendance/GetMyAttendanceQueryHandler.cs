namespace BiometricAttendance.Application.Features.Attendances.GetMyAttendance;

public class GetMyAttendanceQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMyAttendanceQuery, Result<MyAttendanceResponse>>
{
    public async Task<Result<MyAttendanceResponse>> Handle(GetMyAttendanceQuery request, CancellationToken cancellationToken = default)
    {
        if (await unitOfWork.Students.FindAsync(x => x.UserId == request.UserId, cancellationToken) is not { } student)
            return Result.Failure<MyAttendanceResponse>(StudentErrors.NotFound);

        var courseIds = await unitOfWork.StudentCourses
            .FindAllProjectionAsync(x => x.StudentId == student.Id, x => x.CourseId, true, cancellationToken);

        if (!courseIds.Any())
            return Result.Success(new MyAttendanceResponse([], "0.00%"));

        var courses = (await unitOfWork.Courses
            .FindAllAsync(x => courseIds.Contains(x.Id), cancellationToken))
            .ToDictionary(c => c.Id);

        var allAttendances = await unitOfWork.Attendances
            .FindAllAsync(x => courseIds.Contains(x.CourseId), cancellationToken);

        var attendancesByCourse = allAttendances
            .GroupBy(a => a.CourseId)
            .ToDictionary(g => g.Key, g => g.ToList());

        int totalAttendedWeeks = 0;
        int totalWeeksAcrossAllCourses = 0;

        var courseAttendances = courseIds.Select(courseId =>
        {
            attendancesByCourse.TryGetValue(courseId, out var courseAttendancesList);
            courseAttendancesList ??= [];

            var totalWeeks = courseAttendancesList
                .Select(a => a.WeekNumber)
                .Distinct()
                .Count();

            var studentAttendedWeeks = courseAttendancesList
                .Count(a => a.StudentId == student.Id);

            var percentage = totalWeeks > 0
                ? $"{(double)studentAttendedWeeks / totalWeeks * 100:F2}%"
                : "0.00%";

            totalAttendedWeeks += studentAttendedWeeks;
            totalWeeksAcrossAllCourses += totalWeeks;

            courses.TryGetValue(courseId, out var course);

            return new CourseAttendanceResponse(
                courseId,
                course?.Name ?? string.Empty,
                course?.Code ?? string.Empty,
                percentage);
        }).ToList();

        var totalPercentage = totalWeeksAcrossAllCourses > 0
            ? $"{(double)totalAttendedWeeks / totalWeeksAcrossAllCourses * 100:F2}%"
            : "0.00%";

        return Result.Success(new MyAttendanceResponse(courseAttendances, totalPercentage));
    }
}

namespace BiometricAttendance.Application.Features.Attendances.GetTotalAttendance;

public class GetTotalAttendanceQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTotalAttendanceQuery, Result<IEnumerable<TotalAttendanceResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<TotalAttendanceResponse>>> Handle(GetTotalAttendanceQuery request, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.FindAsync(x => x.Id == request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure<IEnumerable<TotalAttendanceResponse>>(CourseErrors.NotFound);

        var attendances = (await _unitOfWork.Attendances.FindAllAsync(
            x => x.CourseId == request.CourseId,
            cancellationToken)).ToList();

        if (attendances.Count == 0)
            return Result.Success(Enumerable.Empty<TotalAttendanceResponse>());

        var totalWeeks = attendances.Select(a => a.WeekNumber).Distinct().Count();

        var studentIds = attendances.Select(a => a.StudentId).Distinct().ToList();

        var students = (await _unitOfWork.Students.FindAllWithNameAsync(
            x => studentIds.Contains(x.Id),
            cancellationToken)).ToDictionary(s => s.Id);

        var response = attendances
            .GroupBy(a => a.StudentId)
            .Select(group =>
            {
                var studentId = group.Key;
                var attendedWeeks = group.Count();
                var percentage = totalWeeks > 0
                    ? $"{(double)attendedWeeks / totalWeeks * 100:F2}%"
                    : "0.00%";

                var studentName = students.TryGetValue(studentId, out var s) ? s.Name ?? s.UserId : studentId.ToString();

                return new TotalAttendanceResponse(studentId, studentName, course.Name, percentage);
            });

        return Result.Success(response);
    }
}

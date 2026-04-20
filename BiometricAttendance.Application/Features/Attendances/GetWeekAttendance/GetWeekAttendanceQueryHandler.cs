namespace BiometricAttendance.Application.Features.Attendances.GetWeekAttendance;

public class GetWeekAttendanceQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetWeekAttendanceQuery, Result<IEnumerable<WeekAttendanceResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<WeekAttendanceResponse>>> Handle(GetWeekAttendanceQuery request, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.FindAsync(x => x.Id == request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure<IEnumerable<WeekAttendanceResponse>>(CourseErrors.NotFound);

        var attendances = (await _unitOfWork.Attendances.FindAllAsync(
            x => x.CourseId == request.CourseId && x.WeekNumber == request.WeekNumber,
            cancellationToken)).ToList();

        if (attendances.Count == 0)
            return Result.Success(Enumerable.Empty<WeekAttendanceResponse>());

        var studentIds = attendances.Select(a => a.StudentId).Distinct().ToList();

        var students = (await _unitOfWork.Students.FindAllWithNameAsync(
            x => studentIds.Contains(x.Id),
            cancellationToken)).ToDictionary(s => s.Id);

        var response = attendances.Select(a =>
        {
            var studentName = students.TryGetValue(a.StudentId, out var s) ? s.Name ?? s.UserId : a.StudentId.ToString();
            return new WeekAttendanceResponse(a.Id, studentName, course.Name, a.WeekNumber, a.MarkedAt);
        });

        return Result.Success(response);
    }
}

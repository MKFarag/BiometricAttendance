namespace BiometricAttendance.Application.Features.Attendances.GetWeekAttendance;

public record GetWeekAttendanceQuery(int CourseId, int WeekNumber) : IRequest<Result<IEnumerable<WeekAttendanceResponse>>>;

namespace BiometricAttendance.Application.Features.Attendances.MarkAttendance;

public record MarkAttendanceCommand(int StudentId, int CourseId, int WeekNumber) : IRequest<Result>;

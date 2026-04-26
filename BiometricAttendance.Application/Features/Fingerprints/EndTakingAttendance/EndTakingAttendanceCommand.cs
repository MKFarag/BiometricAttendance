namespace BiometricAttendance.Application.Features.Fingerprints.EndTakingAttendance;

public record EndTakingAttendanceCommand(int CourseId, int WeekNum) : IRequest<Result>;

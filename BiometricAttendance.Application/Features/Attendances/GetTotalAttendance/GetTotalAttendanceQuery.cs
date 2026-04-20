namespace BiometricAttendance.Application.Features.Attendances.GetTotalAttendance;

public record GetTotalAttendanceQuery(int CourseId) : IRequest<Result<IEnumerable<TotalAttendanceResponse>>>;

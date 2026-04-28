namespace BiometricAttendance.Application.Features.Attendances.GetMyAttendance;

public record GetMyAttendanceQuery(string UserId) : IRequest<Result<MyAttendanceResponse>>;
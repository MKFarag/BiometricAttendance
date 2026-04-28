namespace BiometricAttendance.Application.Features.Profile.GetForStudent;

public record GetStudentProfileQuery(string UserId) : IRequest<Result<StudentProfileResponse>>;

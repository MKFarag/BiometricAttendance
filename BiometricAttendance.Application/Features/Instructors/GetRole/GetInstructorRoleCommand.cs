namespace BiometricAttendance.Application.Features.Instructors.GetRole;

public record GetInstructorRoleCommand(string UserId, string Pass) : IRequest<Result>;

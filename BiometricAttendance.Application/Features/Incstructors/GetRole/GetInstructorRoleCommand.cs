namespace BiometricAttendance.Application.Features.Incstructors.GetRole;

public record GetInstructorRoleCommand(string UserId, string Pass) : IRequest<Result>;

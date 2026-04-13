namespace BiometricAttendance.Application.Features.Roles.Update;

public record UpdateRoleCommand(string Id, RoleRequest Request) : IRequest<Result>;

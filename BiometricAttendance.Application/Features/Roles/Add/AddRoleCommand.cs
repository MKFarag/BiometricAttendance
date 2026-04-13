namespace BiometricAttendance.Application.Features.Roles.Add;

public record AddRoleCommand(RoleRequest Request) : IRequest<Result<RoleDetailResponse>>;

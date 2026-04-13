namespace BiometricAttendance.Application.Contracts.Roles;

public record RoleDetailResponse(
    string Id,
    string Name,
    bool IsDisabled,
    IEnumerable<string> Permissions
);

namespace BiometricAttendance.Application.Contracts.Roles;

public record RoleResponse(
    string Id,
    string Name,
    bool IsDisabled
);

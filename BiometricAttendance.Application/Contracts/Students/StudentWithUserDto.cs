namespace BiometricAttendance.Application.Contracts.Students;

public record StudentWithUserDto(
    int Id,
    string Name,
    string Email,
    int Level,
    string DepartmentName
);

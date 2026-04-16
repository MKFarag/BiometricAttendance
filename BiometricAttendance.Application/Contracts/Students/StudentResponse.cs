namespace BiometricAttendance.Application.Contracts.Students;

public record StudentResponse(
    int Id,
    string Name,
    int Level,
    string DepartmentName
);

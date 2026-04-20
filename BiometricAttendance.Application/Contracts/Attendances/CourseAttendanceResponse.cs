namespace BiometricAttendance.Application.Contracts.Attendances;

public record CourseAttendanceResponse(
    int Id,
    string Name,
    string Code,
    string TotalAttendancePercentage
);

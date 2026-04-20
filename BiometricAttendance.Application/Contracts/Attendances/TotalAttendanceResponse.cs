namespace BiometricAttendance.Application.Contracts.Attendances;

public record TotalAttendanceResponse(
    int Id,
    string StudentName,
    string CourseName,
    string TotalAttendancePercentage
);

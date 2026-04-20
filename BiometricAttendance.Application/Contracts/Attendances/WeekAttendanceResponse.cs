namespace BiometricAttendance.Application.Contracts.Attendances;

public record WeekAttendanceResponse(
    int Id,
    string StudentName,
    string CourseName,
    int WeekNumber,
    DateTime? MarkedAt
);

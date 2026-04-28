namespace BiometricAttendance.Application.Contracts.Attendances;

public record MyAttendanceResponse(
    List<CourseAttendanceResponse> Attendances,
    string TotalPercentage
);

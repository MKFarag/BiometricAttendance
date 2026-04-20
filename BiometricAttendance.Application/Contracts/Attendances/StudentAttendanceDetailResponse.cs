namespace BiometricAttendance.Application.Contracts.Attendances;

public record StudentAttendanceDetailResponse(
    int Id,
    StudentResponse Student,
    CourseAttendanceResponse CourseAttendance
);
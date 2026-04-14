namespace BiometricAttendance.Application.Contracts.Departments;

public record DepartmentDetailResponse(
    int Id,
    string Name,
    int StudentsCount,
    int CoursesCount
);

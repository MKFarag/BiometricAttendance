namespace BiometricAttendance.Application.Contracts.Courses;

public record CourseDetailResponse(
    int Id,
    string Name,
    string Code,
    DepartmentResponse Department
);

namespace BiometricAttendance.Application.Contracts.Courses;

public record CourseDetailResponse(
    int Id,
    string Name,
    string Code,
    int Level,
    IList<DepartmentResponse> Departments
);

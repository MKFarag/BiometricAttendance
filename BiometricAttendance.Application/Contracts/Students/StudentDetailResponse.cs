namespace BiometricAttendance.Application.Contracts.Students;

public record StudentDetailResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    int Level,
    DepartmentResponse Department,
    IList<CourseResponse> Courses
);

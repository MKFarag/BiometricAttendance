namespace BiometricAttendance.Application.Features.Students.EnrollCourses;

public record EnrollStudentCoursesCommand(string UserId, IEnumerable<int> CoursesId) : IRequest<Result>;

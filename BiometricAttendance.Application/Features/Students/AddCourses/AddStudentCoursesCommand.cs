namespace BiometricAttendance.Application.Features.Students.AddCourses;

public record AddStudentCoursesCommand(string UserId, IEnumerable<int> CoursesId) : IRequest<Result>;

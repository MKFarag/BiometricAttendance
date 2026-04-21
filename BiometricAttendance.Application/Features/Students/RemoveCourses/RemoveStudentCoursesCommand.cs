namespace BiometricAttendance.Application.Features.Students.RemoveCourses;

public record RemoveStudentCoursesCommand(string UserId, IEnumerable<int> CoursesId) : IRequest<Result>;

namespace BiometricAttendance.Application.Features.Courses.Add;

public record AddCourseCommand(string Name, string Code, int DepartmentId) : IRequest<Result<CourseResponse>>;

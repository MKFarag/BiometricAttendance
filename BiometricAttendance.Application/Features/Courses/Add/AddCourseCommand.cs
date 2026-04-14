namespace BiometricAttendance.Application.Features.Courses.Add;

public record AddCourseCommand(string Name, string Code, int Level) : IRequest<Result<CourseResponse>>;

namespace BiometricAttendance.Application.Features.Courses.Update;

public record UpdateCourseCommand(int Id, string Name, string Code, int DepartmentId) : IRequest<Result>;

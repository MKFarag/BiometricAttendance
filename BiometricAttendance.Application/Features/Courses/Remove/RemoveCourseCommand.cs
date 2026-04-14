namespace BiometricAttendance.Application.Features.Courses.Remove;

public record RemoveCourseCommand(int Id) : IRequest<Result>;

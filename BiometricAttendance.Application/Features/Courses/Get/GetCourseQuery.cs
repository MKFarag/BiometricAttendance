namespace BiometricAttendance.Application.Features.Courses.Get;

public record GetCourseQuery(int Id) : IRequest<Result<CourseDetailResponse>>;

namespace BiometricAttendance.Application.Features.Courses.GetAll;

public record GetAllCoursesQuery : IRequest<IEnumerable<CourseResponse>>;

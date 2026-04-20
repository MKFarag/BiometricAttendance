namespace BiometricAttendance.Application.Features.Attendances.GetStudentAttendanceDetail;

public record GetStudentAttendanceDetailQuery(int StudentId, int CourseId) : IRequest<Result<StudentAttendanceDetailResponse>>;

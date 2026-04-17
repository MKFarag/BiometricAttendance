namespace BiometricAttendance.Application.Features.Students.Get;

public record GetStudentQuery(int Id) : IRequest<Result<StudentDetailResponse>>;

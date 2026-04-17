namespace BiometricAttendance.Application.Features.Students.Promote;

public record PromoteStudentCommand(int Id) : IRequest<Result>;
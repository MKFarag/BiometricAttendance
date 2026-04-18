namespace BiometricAttendance.Application.Features.Students.Promote;

public record PromoteStudentCommand(int StudentId) : IRequest<Result>;
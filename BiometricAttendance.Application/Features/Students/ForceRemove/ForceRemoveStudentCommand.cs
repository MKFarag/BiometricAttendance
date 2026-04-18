namespace BiometricAttendance.Application.Features.Students.ForceRemove;

public record ForceRemoveStudentCommand(int StudentId) : IRequest<Result>;

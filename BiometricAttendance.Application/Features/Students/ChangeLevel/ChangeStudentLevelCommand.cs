namespace BiometricAttendance.Application.Features.Students.ChangeLevel;

public record ChangeStudentLevelCommand(int StudentId, int Level) : IRequest<Result>;

namespace BiometricAttendance.Application.Features.Students.ChangeDepartment;

public record ChangeStudentDepartmentCommand(int StudentId, int DepartmentId) : IRequest<Result>;

namespace BiometricAttendance.Application.Features.Departments.Update;

public record UpdateDepartmentCommand(int Id, string Name) : IRequest<Result>;

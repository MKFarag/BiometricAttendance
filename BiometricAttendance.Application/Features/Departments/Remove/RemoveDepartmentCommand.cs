namespace BiometricAttendance.Application.Features.Departments.Remove;

public record RemoveDepartmentCommand(int Id) : IRequest<Result>;

namespace BiometricAttendance.Application.Features.Departments.Add;

public record AddDepartmentCommand(string Name) : IRequest<Result<DepartmentResponse>>;

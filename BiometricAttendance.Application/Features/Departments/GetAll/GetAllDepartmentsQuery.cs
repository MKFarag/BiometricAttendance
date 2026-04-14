namespace BiometricAttendance.Application.Features.Departments.GetAll;

public record GetAllDepartmentsQuery : IRequest<IEnumerable<DepartmentResponse>>;

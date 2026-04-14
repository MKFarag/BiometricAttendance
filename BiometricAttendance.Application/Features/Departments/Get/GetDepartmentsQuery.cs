namespace BiometricAttendance.Application.Features.Departments.Get;

public record GetDepartmentsQuery(int Id) : IRequest<Result<DepartmentDetailResponse>>;

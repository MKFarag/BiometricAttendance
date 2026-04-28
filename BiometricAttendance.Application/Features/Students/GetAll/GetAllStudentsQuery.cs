namespace BiometricAttendance.Application.Features.Students.GetAll;

public record GetAllStudentsQuery(RequestFilters Filters) : IRequest<IPaginatedList<StudentResponse>>;

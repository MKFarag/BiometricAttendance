using System;
using System.Collections.Generic;
using System.Text;

namespace BiometricAttendance.Application.Features.Students.GetAll;

public record GetAllStudentsQuery(RequestFilters Filters) : IRequest<IPaginatedList<StudentResponse>>;

namespace BiometricAttendance.Application.Features.Departments.Get;

public class GetDepartmentsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDepartmentsQuery, Result<DepartmentDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<DepartmentDetailResponse>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Departments.GetAsync([request.Id], cancellationToken) is not { } department)
            return Result.Failure<DepartmentDetailResponse>(DepartmentErrors.NotFound);

        var studentsCount = await _unitOfWork.Students.CountAsync(x => x.DepartmentId == department.Id, cancellationToken);
        var coursesCount = await _unitOfWork.DepartmentCourses.CountAsync(x => x.DepartmentId == department.Id, cancellationToken);

        var response = new DepartmentDetailResponse(department.Id, department.Name, studentsCount, coursesCount);

        return Result.Success(response);
    }
}

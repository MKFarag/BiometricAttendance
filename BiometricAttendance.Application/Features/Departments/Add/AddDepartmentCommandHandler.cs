namespace BiometricAttendance.Application.Features.Departments.Add;

public class AddDepartmentCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<AddDepartmentCommand, Result<DepartmentResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<DepartmentResponse>> Handle(AddDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Departments.AnyAsync(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken))
            return Result.Failure<DepartmentResponse>(DepartmentErrors.NameAlreadyExists);

        var department = new Department { Name = request.Name };

        await _unitOfWork.Departments.AddAsync(department, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cacheService.RemoveByTagAsync(Cache.Tags.Departments, cancellationToken);

        return Result.Success(department.Adapt<DepartmentResponse>());
    }
}

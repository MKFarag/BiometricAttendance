namespace BiometricAttendance.Application.Features.Departments.Update;

public class UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<UpdateDepartmentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Departments.GetAsync([request.Id], cancellationToken) is not { } department)
            return Result.Failure(DepartmentErrors.NotFound);

        if (string.Equals(request.Name, department.Name, StringComparison.OrdinalIgnoreCase))
            return Result.Success();

        if (await _unitOfWork.Departments.AnyAsync(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken))
            return Result.Failure(DepartmentErrors.NameAlreadyExists);

        department = request.Adapt(department);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cacheService.RemoveByTagAsync(Cache.Tags.Departments, cancellationToken);

        return Result.Success();
    }
}

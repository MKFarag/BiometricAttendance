namespace BiometricAttendance.Application.Features.Departments.Remove;

public class RemoveDepartmentCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService) : IRequestHandler<RemoveDepartmentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result> Handle(RemoveDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Departments.AnyAsync(x => x.Id == request.Id, cancellationToken))
            return Result.Failure(DepartmentErrors.NotFound);

        if (await _unitOfWork.Courses.AnyAsync(x => x.DepartmentId == request.Id, cancellationToken)
            || await _unitOfWork.Students.AnyAsync(x => x.DepartmentId == request.Id, cancellationToken))
            return Result.Failure(DepartmentErrors.InUse);

        await _unitOfWork.Departments.ExecuteDeleteAsync(x => x.Id == request.Id, cancellationToken);

        await _cacheService.RemoveByTagAsync(Cache.Tags.Departments, cancellationToken);

        return Result.Success();
    }
}

namespace BiometricAttendance.Application.Features.Students.Promote;

public class PromoteStudentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<PromoteStudentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(PromoteStudentCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync([request.Id], cancellationToken) is not { } student)
            return Result.Failure(StudentErrors.NotFound);

        if (!student.CanPromote)
            return Result.Failure(StudentErrors.PromoteFailed);

        student.Promote();

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
